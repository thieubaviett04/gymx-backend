using System.Net.Http.Headers;
using System.Text.Json;
using GymX.Application.Common.Interfaces;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
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
        var detectResult = await DetectFaceAsync(imageBytes, cancellationToken);
        if (detectResult.IsFailure)
        {
            return Result<string>.Failure(detectResult.Error);
        }

        var temporaryFaceToken = detectResult.Value;

        if (!string.IsNullOrWhiteSpace(_options.FaceSetToken))
        {
            try
            {
                var addFaceUrl = $"{_options.BaseUrl}/facepp/v3/faceset/addface";
                using var addContent = new MultipartFormDataContent();
                addContent.AddQuotedField("api_key", _options.ApiKey);
                addContent.AddQuotedField("api_secret", _options.ApiSecret);
                addContent.AddQuotedField("faceset_token", _options.FaceSetToken);
                addContent.AddQuotedField("face_tokens", temporaryFaceToken);

                var addResponse = await httpClient.PostAsync(addFaceUrl, addContent, cancellationToken);
                var addResponseBody = await addResponse.Content.ReadAsStringAsync(cancellationToken);
                logger.LogInformation("Face++ AddFace result: {StatusCode} {Response}", (int)addResponse.StatusCode, addResponseBody);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Lỗi khi gọi Face++ AddFace nhưng face_token đã phát hiện thành công.");
            }
        }

        return Result<string>.Success(temporaryFaceToken);
    }

    public async Task<Result<string>> IdentifyFaceAsync(byte[] imageBytes, CancellationToken cancellationToken = default)
    {
        var detectResult = await DetectFaceAsync(imageBytes, cancellationToken);
        if (detectResult.IsFailure)
        {
            return Result<string>.Failure(detectResult.Error);
        }

        var faceToken = detectResult.Value;
        var searchUrl = $"{_options.BaseUrl}/facepp/v3/search";

        using var searchContent = new MultipartFormDataContent();
        searchContent.AddQuotedField("api_key", _options.ApiKey);
        searchContent.AddQuotedField("api_secret", _options.ApiSecret);
        searchContent.AddQuotedField("faceset_token", _options.FaceSetToken);
        searchContent.AddQuotedField("face_token", faceToken);

        var searchResponse = await httpClient.PostAsync(searchUrl, searchContent, cancellationToken);
        var searchResponseBody = await searchResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!searchResponse.IsSuccessStatusCode)
        {
            logger.LogError("Face++ Search API error {StatusCode}: {Response}", (int)searchResponse.StatusCode, searchResponseBody);
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

        logger.LogInformation("Tìm thấy khuôn mặt khớp. FaceToken: {Token}, Độ chính xác: {Confidence}%", matchedFaceToken, confidence);
        return Result<string>.Success(matchedFaceToken);
    }

    private async Task<Result<string>> DetectFaceAsync(byte[] imageBytes, CancellationToken cancellationToken)
    {
        var detectUrl = $"{_options.BaseUrl}/facepp/v3/detect";

        using var detectContent = new MultipartFormDataContent();
        detectContent.AddQuotedField("api_key", _options.ApiKey);
        detectContent.AddQuotedField("api_secret", _options.ApiSecret);
        detectContent.AddQuotedFile("image_file", "face.jpg", imageBytes, "image/jpeg");

        var detectResponse = await httpClient.PostAsync(detectUrl, detectContent, cancellationToken);
        var detectResponseBody = await detectResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!detectResponse.IsSuccessStatusCode)
        {
            logger.LogError("Face++ Detect API error {StatusCode}: {Response}", (int)detectResponse.StatusCode, detectResponseBody);
            return Result<string>.Failure(Error.Failure(
                "FaceRecognition.DetectFailed",
                $"Lỗi kết nối tới dịch vụ Face++. Chi tiết: {detectResponseBody}"));
        }

        using var doc = JsonDocument.Parse(detectResponseBody);
        var faces = doc.RootElement.GetProperty("faces");

        if (faces.GetArrayLength() == 0)
        {
            logger.LogWarning("Face++ không phát hiện khuôn mặt nào trong ảnh.");
            return Result<string>.Failure(EmployeeFaceErrors.NoFaceFound);
        }

        if (faces.GetArrayLength() > 1)
        {
            logger.LogWarning("Face++ phát hiện {Count} khuôn mặt trong ảnh.", faces.GetArrayLength());
            return Result<string>.Failure(EmployeeFaceErrors.MultipleFacesDetected);
        }

        return Result<string>.Success(faces[0].GetProperty("face_token").GetString()!);
    }
}

internal static class MultipartFormDataContentExtensions
{
    public static void AddQuotedField(this MultipartFormDataContent form, string name, string value)
    {
        var content = new StringContent(value ?? string.Empty);
        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{name}\""
        };
        content.Headers.ContentType = null;
        form.Add(content);
    }

    public static void AddQuotedFile(this MultipartFormDataContent form, string name, string fileName, byte[] bytes, string contentType = "image/jpeg")
    {
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{name}\"",
            FileName = $"\"{fileName}\""
        };
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(content);
     }
}
