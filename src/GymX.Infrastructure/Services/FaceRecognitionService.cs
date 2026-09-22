using System.Text.Json;
using GymX.Application.Common.Interfaces;
using GymX.Domain.Common.Models;
using GymX.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GymX.Infrastructure.Services;

public class FaceRecognitionService(
    HttpClient httpClient,
    IOptions<FacePlusPlusOptions> options,
    ILogger<FaceRecognitionService> logger) : IFaceRecognitionService
{
    private readonly FacePlusPlusOptions _options = options.Value;

    public async Task<Result<string>> RegisterFaceAsync(byte[] imageBytes, CancellationToken cancellationToken = default)
    {
        // --- BƯỚC 1: Phát hiện khuôn mặt trong ảnh (Detect) ---
        var detectResult = await DetectFaceAsync(imageBytes, cancellationToken);
        if (detectResult.IsFailure)
            return Result<string>.Failure(detectResult.Error);

        var temporaryFaceToken = detectResult.Value;

        // --- BƯỚC 2: Lưu face_token vào FaceSet (AddFace) vĩnh viễn ---
        var addFaceUrl = $"{_options.BaseUrl}/facepp/v3/faceset/addface";

        using var addContent = new MultipartFormDataContent();
        addContent.AddField("api_key", _options.ApiKey);
        addContent.AddField("api_secret", _options.ApiSecret);
        addContent.AddField("faceset_token", _options.FaceSetToken);
        addContent.AddField("face_tokens", temporaryFaceToken);

        var addResponse = await httpClient.PostAsync(addFaceUrl, addContent, cancellationToken);
        var addResponseBody = await addResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!addResponse.IsSuccessStatusCode)
        {
            logger.LogError("Face++ AddFace API lỗi {StatusCode}: {Response}",
                (int)addResponse.StatusCode, addResponseBody);

            return Result<string>.Failure(Error.Failure(
                "FaceRecognition.AddFaceFailed",
                $"Không thể lưu khuôn mặt vào hệ thống Face++. Chi tiết: {addResponseBody}"));
        }

        logger.LogInformation("Đăng ký khuôn mặt thành công. FaceToken: {Token}", temporaryFaceToken);
        return Result<string>.Success(temporaryFaceToken);
    }

    public async Task<Result<string>> IdentifyFaceAsync(byte[] imageBytes, CancellationToken cancellationToken = default)
    {
        // --- BƯỚC 1: Phát hiện khuôn mặt trong ảnh từ Camera ---
        var detectResult = await DetectFaceAsync(imageBytes, cancellationToken);
        if (detectResult.IsFailure)
            return Result<string>.Failure(detectResult.Error);

        var faceToken = detectResult.Value;

        // --- BƯỚC 2: Tìm kiếm trong FaceSet của GymX (Search) ---
        var searchUrl = $"{_options.BaseUrl}/facepp/v3/search";

        using var searchContent = new MultipartFormDataContent();
        searchContent.AddField("api_key", _options.ApiKey);
        searchContent.AddField("api_secret", _options.ApiSecret);
        searchContent.AddField("faceset_token", _options.FaceSetToken);
        searchContent.AddField("face_token", faceToken);

        var searchResponse = await httpClient.PostAsync(searchUrl, searchContent, cancellationToken);
        var searchResponseBody = await searchResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!searchResponse.IsSuccessStatusCode)
        {
            logger.LogError("Face++ Search API lỗi {StatusCode}: {Response}",
                (int)searchResponse.StatusCode, searchResponseBody);

            return Result<string>.Failure(Error.Failure(
                "FaceRecognition.SearchFailed",
                $"Lỗi khi tìm kiếm khuôn mặt. Chi tiết: {searchResponseBody}"));
        }

        using var doc = JsonDocument.Parse(searchResponseBody);
        var results = doc.RootElement.GetProperty("results");

        if (results.GetArrayLength() == 0)
        {
            logger.LogWarning("Face++ không tìm thấy khuôn mặt khớp trong FaceSet.");
            return Result<string>.Failure(Error.NotFound(
                "FaceRecognition.NotFound",
                "Khuôn mặt này chưa được đăng ký trong hệ thống. Vui lòng liên hệ Lễ tân."));
        }

        var matchedFaceToken = results[0].GetProperty("face_token").GetString()!;
        var confidence = results[0].GetProperty("confidence").GetDouble();

        logger.LogInformation("Tìm thấy khuôn mặt khớp. FaceToken: {Token}, Độ chính xác: {Confidence}%",
            matchedFaceToken, confidence);

        return Result<string>.Success(matchedFaceToken);
    }

    // --- Hàm nội bộ dùng chung ---
    private async Task<Result<string>> DetectFaceAsync(byte[] imageBytes, CancellationToken cancellationToken)
    {
        var detectUrl = $"{_options.BaseUrl}/facepp/v3/detect";

        using var detectContent = new MultipartFormDataContent();
        detectContent.AddField("api_key", _options.ApiKey);
        detectContent.AddField("api_secret", _options.ApiSecret);
        // Ảnh dùng ByteArrayContent thay StringContent
        detectContent.Add(new ByteArrayContent(imageBytes), "image_file", "face.jpg");

        var detectResponse = await httpClient.PostAsync(detectUrl, detectContent, cancellationToken);
        var detectResponseBody = await detectResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!detectResponse.IsSuccessStatusCode)
        {
            logger.LogError("Face++ Detect API lỗi {StatusCode}: {Response}",
                (int)detectResponse.StatusCode, detectResponseBody);

            return Result<string>.Failure(Error.Failure(
                "FaceRecognition.DetectFailed",
                $"Lỗi kết nối tới dịch vụ Face++. Chi tiết: {detectResponseBody}"));
        }

        using var doc = JsonDocument.Parse(detectResponseBody);
        var faces = doc.RootElement.GetProperty("faces");

        if (faces.GetArrayLength() == 0)
        {
            logger.LogWarning("Face++ không phát hiện khuôn mặt nào trong ảnh.");
            return Result<string>.Failure(Error.Validation(
                "FaceRecognition.NoFaceFound",
                "Không tìm thấy khuôn mặt nào trong ảnh. Vui lòng chụp lại với ánh sáng tốt hơn."));
        }

        return Result<string>.Success(faces[0].GetProperty("face_token").GetString()!);
    }
}

/// <summary>
/// Extension method để thêm string field vào MultipartFormDataContent
/// Dùng ByteArrayContent thay StringContent để tránh hoàn toàn Content-Type header.
/// </summary>
internal static class MultipartFormDataContentExtensions
{
    public static void AddField(this MultipartFormDataContent form, string name, string value)
    {
        // Dùng ByteArrayContent thay StringContent vì StringContent tự gắn "Content-Type: text/plain; charset=utf-8"
        // khiến một số API (bao gồm Face++) không đọc được field và báo MISSING_ARGUMENTS
        var bytes = System.Text.Encoding.UTF8.GetBytes(value);
        form.Add(new ByteArrayContent(bytes), name);
    }
}
