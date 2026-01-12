using Application.Core;
using Application.Interfaces;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

public class DeletePhoto
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string PhotoId { get; set; }
    }

    public class Handler(AppDbContext dbContext, IUserAccessor userAccessor, IPhotoService photoService)
        : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userAccessor.GetUserWithPhotosAsync();

            var photo = user.Photos.FirstOrDefault(x => x.Id == request.PhotoId);

            if (photo == null) return Result<Unit>.Failure("Cannot find photo", 400);

            if (photo.Url == user.ImageUrl) return Result<Unit>.Failure("Cannot delete main photo", 400);

            await photoService.DeletePhoto(photo.PublicId);

            user.Photos.Remove(photo);

            var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Problem deleting photo", 400);
        }
    }
}