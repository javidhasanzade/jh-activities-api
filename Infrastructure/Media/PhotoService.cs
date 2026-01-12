using Application.Interfaces;
using Application.Profiles.DTOs;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Media;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> cloudinaryOptions)
    {
        var account = new Account(cloudinaryOptions.Value.CloudName, cloudinaryOptions.Value.ApiKey,
            cloudinaryOptions.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<PhotoUploadResult?> UploadPhoto(IFormFile file)
    {
        if (file.Length <= 0) return null;

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            // Transformation = new Transformation().Height(500).Width(500).Crop("fill"),
            Folder = "JH-Activities-API"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null) throw new Exception(uploadResult.Error.Message);

        return new PhotoUploadResult
        {
            PublicId = uploadResult.PublicId,
            Url = uploadResult.Url.AbsoluteUri
        };
    }

    public async Task<string> DeletePhoto(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Error != null ? throw new Exception(result.Error.Message) : result.Result;
    }
}