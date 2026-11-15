using GymX.Domain.Common.Models;

namespace GymX.Application.Common.Interfaces;

public interface IFaceRecognitionService
{
    Task<Result<string>> RegisterFaceAsync(byte[] imageBytes, CancellationToken cancellationToken = default);

    Task<Result<string>> IdentifyFaceAsync(
        byte[] imageBytes,
        IEnumerable<string>? candidateFaceTokens = null,
        CancellationToken cancellationToken = default);

    Task<Result<double>> CompareFacesAsync(
        string faceToken1,
        string faceToken2,
        CancellationToken cancellationToken = default);
}
