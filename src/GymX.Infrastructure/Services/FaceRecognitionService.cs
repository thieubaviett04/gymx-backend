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
                logger.LogWarning(ex, "Lỗi khi gọi Face++ AddFace");
            }
        }

        return Result<string>.Success(temporaryFaceToken);
    }

    public async Task<Result<string>> IdentifyFaceAsync(
        byte[] imageBytes,
        IEnumerable<string>? candidateFaceTokens = null,
        CancellationToken cancellationToken = default)
    {
        var detectResult = await DetectFaceAsync(imageBytes, cancellationToken);
        if (detectResult.IsFailure)
        {
            return Result<string>.Failure(detectResult.Error);
        }

        var faceToken = detectResult.Value;

        var candidatesList = candidateFaceTokens?
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();

        if (candidatesList is not null && candidatesList.Count > 0)
        {
            string? bestMatchToken = null;
            double maxConfidence = 0;

            foreach (var candidate in candidatesList)
            {
                var compareResult = await CompareFacesAsync(faceToken, candidate, cancellationToken);
                if (compareResult.IsSuccess)
                {
                    var confidence = compareResult.Value;
                    if (confidence > maxConfidence)
                    {
                        maxConfidence = confidence;
                        bestMatchToken = candidate;
                    }
                }
            }

            const double confidenceThreshold = 82.0;
            if (bestMatchToken is not null && maxConfidence >= confidenceThreshold)
            {
                logger.LogInformation("Khuôn mặt khớp {Token} với độ tin cậy {Confidence}% (vượt ngưỡng {Threshold}%)",
                    bestMatchToken, maxConfidence, confidenceThreshold);
                return Result<string>.Success(bestMatchToken);
            }

            logger.LogWarning("Không tìm thấy khuôn mặt khớp (Max confidence: {Confidence}%, Ngưỡng yêu cầu: {Threshold}%)",
                maxConfidence, confidenceThreshold);
            return Result<string>.Failure(EmployeeFaceErrors.NoFaceFound);
        }

        if (!string.IsNullOrWhiteSpace(_options.FaceSetToken))
        {
            var searchUrl = $"{_options.BaseUrl}/facepp/v3/search";

            using var searchContent = new MultipartFormDataContent();
            searchContent.AddQuotedField("api_key", _options.ApiKey);
            searchContent.AddQuotedField("api_secret", _options.ApiSecret);
            searchContent.AddQuotedField("faceset_token", _options.FaceSetToken);
            searchContent.AddQuotedField("face_token", faceToken);

            var searchResponse = await httpClient.PostAsync(searchUrl, searchContent, cancellationToken);
            var searchResponseBody = await searchResponse.Content.ReadAsStringAsync(cancellationToken);

            if (searchResponse.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(searchResponseBody);
                if (doc.RootElement.TryGetProperty("results", out var results) && results.GetArrayLength() > 0)
                {
                    var matchedFaceToken = results[0].GetProperty("face_token").GetString()!;
                    var confidence = results[0].GetProperty("confidence").GetDouble();
                    logger.LogInformation("Tìm thấy khuôn mặt khớp trong FaceSet. FaceToken: {Token}, Độ chính xác: {Confidence}%", matchedFaceToken, confidence);
                    return Result<string>.Success(matchedFaceToken);
                }
            }
        }

        return Result<string>.Failure(EmployeeFaceErrors.NoFaceFound);
    }

    public async Task<Result<double>> CompareFacesAsync(
        string faceToken1,
        string faceToken2,
        CancellationToken cancellationToken = default)
    {
        var compareUrl = $"{_options.BaseUrl}/facepp/v3/compare";

        for (int retry = 0; retry < 3; retry++)
        {
            using var compareContent = new MultipartFormDataContent();
            compareContent.AddQuotedField("api_key", _options.ApiKey);
            compareContent.AddQuotedField("api_secret", _options.ApiSecret);
            compareContent.AddQuotedField("face_token1", faceToken1);
            compareContent.AddQuotedField("face_token2", faceToken2);

            var response = await httpClient.PostAsync(compareUrl, compareContent, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("confidence", out var confProp))
                {
                    return Result<double>.Success(confProp.GetDouble());
                }
            }

            if (responseBody.Contains("CONCURRENCY_LIMIT_EXCEEDED"))
            {
                await Task.Delay(400 * (retry + 1), cancellationToken);
                continue;
            }

            logger.LogWarning("Face++ Compare error {StatusCode}: {Response}", (int)response.StatusCode, responseBody);
            break;
        }

        return Result<double>.Failure(Error.Failure("FaceRecognition.CompareFailed", "Lỗi khi so sánh khuôn mặt"));
    }

    private async Task<Result<string>> DetectFaceAsync(byte[] imageBytes, CancellationToken cancellationToken)
    {
        var detectUrl = $"{_options.BaseUrl}/facepp/v3/detect";

        using var detectContent = new MultipartFormDataContent();
        detectContent.AddQuotedField("api_key", _options.ApiKey);
        detectContent.AddQuotedField("api_secret", _options.ApiSecret);
        detectContent.AddQuotedField("return_attributes", "blur,facequality");
        detectContent.AddQuotedFile("image_file", "face.jpg", imageBytes, "image/jpeg");

        var detectResponse = await httpClient.PostAsync(detectUrl, detectContent, cancellationToken);
        var detectResponseBody = await detectResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!detectResponse.IsSuccessStatusCode)
        {
            logger.LogError("Face++ Detect API error {StatusCode}: {Response}", (int)detectResponse.StatusCode, detectResponseBody);
            return Result<string>.Failure(EmployeeFaceErrors.DetectFailed);
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

        var face = faces[0];

        if (face.TryGetProperty("attributes", out var attributes))
        {
            if (attributes.TryGetProperty("blur", out var blur))
            {
                var isBlurry = false;
                if (blur.TryGetProperty("blurness", out var blurness) &&
                    blurness.TryGetProperty("value", out var blurVal) &&
                    blurness.TryGetProperty("threshold", out var blurThresh))
                {
                    if (blurVal.GetDouble() > blurThresh.GetDouble())
                    {
                        isBlurry = true;
                    }
                }

                if (blur.TryGetProperty("motionblur", out var motion) &&
                    motion.TryGetProperty("value", out var motionVal) &&
                    motion.TryGetProperty("threshold", out var motionThresh))
                {
                    if (motionVal.GetDouble() > motionThresh.GetDouble())
                    {
                        isBlurry = true;
                    }
                }

                if (isBlurry)
                {
                    logger.LogWarning("Face++ phát hiện ảnh bị mờ hoặc rung lắc.");
                    return Result<string>.Failure(EmployeeFaceErrors.FaceImageBlurry);
                }
            }

            if (attributes.TryGetProperty("facequality", out var facequality) &&
                facequality.TryGetProperty("value", out var qualityVal) &&
                facequality.TryGetProperty("threshold", out var qualityThresh))
            {
                if (qualityVal.GetDouble() < qualityThresh.GetDouble())
                {
                    logger.LogWarning("Face++ phát hiện chất lượng khuôn mặt thấp ({Quality} < {Threshold}).",
                        qualityVal.GetDouble(), qualityThresh.GetDouble());
                    return Result<string>.Failure(EmployeeFaceErrors.FaceQualityLow);
                }
            }
        }

        return Result<string>.Success(face.GetProperty("face_token").GetString()!);
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
