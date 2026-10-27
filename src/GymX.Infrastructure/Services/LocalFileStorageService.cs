using GymX.Application.Common.Interfaces;

namespace GymX.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _webRootPath;

    public LocalFileStorageService()
    {
        _webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName, CancellationToken cancellationToken = default)
    {
        var targetFolder = Path.Combine(_webRootPath, "uploads", folderName);

        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(targetFolder, uniqueFileName);

        await using var outputStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputStream, cancellationToken);

        return $"/uploads/{folderName}/{uniqueFileName}";
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return Task.CompletedTask;

        var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_webRootPath, relativePath);

        if (File.Exists(fullPath))
        {
            try
            {
                File.Delete(fullPath);
            }
            catch
            {
            }
        }

        return Task.CompletedTask;
    }
}
