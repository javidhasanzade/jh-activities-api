using Application.Core;
using Application.Interfaces;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetProfile
{
    public class Query : IRequest<Result<UserProfileDto>>
    {
        public required string UserId { get; set; }
    }

    public class Handler(AppDbContext dbContext, IMapper mapper, IUserAccessor userAccessor)
        : IRequestHandler<Query, Result<UserProfileDto>>
    {
        public async Task<Result<UserProfileDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var profile = await dbContext.Users
                .ProjectTo<UserProfileDto>(mapper.ConfigurationProvider,
                    new { currentUserId = userAccessor.GetUserId() })
                .SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            return profile == null
                ? Result<UserProfileDto>.Failure("Profile not found", 404)
                : Result<UserProfileDto>.Success(profile);
        }
    }
}