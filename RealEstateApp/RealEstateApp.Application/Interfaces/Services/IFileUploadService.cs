using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Application.Interfaces.Services;

public interface IFileUploadService
{
    Task<string> UploadImageAsync(IFormFile file, string folder);
    Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folder);
    Task<bool> DeleteImageAsync(string imagePath);
    Task<bool> DeleteImagesAsync(List<string> imagePaths);
    bool IsValidImage(IFormFile file);
    string GetImageUrl(string relativePath);
}
