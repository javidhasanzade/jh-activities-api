using Application.Interfaces;
using Application.Profiles.DTOs;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Media;

public class PhotoService : IPhotoService
{
    public Task<PhotoUploadResult> UploadPhoto(IFormFile file)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeletePhoto(string publicId)
    {
        throw new NotImplementedException();
    }
}