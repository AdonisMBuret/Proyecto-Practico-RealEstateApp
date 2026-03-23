using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Shared.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IHostingEnvironment _webHostEnvironment;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private const long MaxFileSize = 5 * 1024 * 1024; 

    public FileUploadService(IHostingEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("El archivo no puede estar vacío");

        if (!IsValidImage(file))
            throw new ArgumentException("El archivo no es una imagen válida");

        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", folder);
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return Path.Combine("images", folder, uniqueFileName).Replace("\\", "/");
    }

    public async Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folder)
    {
        var uploadedFiles = new List<string>();

        foreach (var file in files)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    var filePath = await UploadImageAsync(file, folder);
                    uploadedFiles.Add(filePath);
                }
                catch
                {
                    
                    continue;
                }
            }
        }

        return uploadedFiles;
    }

    public Task<bool> DeleteImageAsync(string imagePath)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath))
                return Task.FromResult(false);

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.Replace("/", "\\"));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<bool> DeleteImagesAsync(List<string> imagePaths)
    {
        var allDeleted = true;

        foreach (var imagePath in imagePaths)
        {
            var deleted = await DeleteImageAsync(imagePath);
            if (!deleted)
                allDeleted = false;
        }

        return allDeleted;
    }

    public bool IsValidImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        if (file.Length > MaxFileSize)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        return _allowedExtensions.Contains(extension);
    }

    public string GetImageUrl(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            return "/images/no-image.png"; 

        return $"/{relativePath.Replace("\\", "/")}";
    }
}
