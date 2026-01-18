using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    public class Query : IRequest<Result<ActivityDto>>
    {
        public required string Id { get; init; }
    }

    public class Handler(AppDbContext dbContext, IMapper mapper, IUserAccessor userAccessor)
        : IRequestHandler<Query, Result<ActivityDto>>
    {
        public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var activity = await dbContext.Activities
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider, new { currentUserId = userAccessor.GetUserId() })
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return activity == null
                ? Result<ActivityDto>.Failure("Activity not found", 404)
                : Result<ActivityDto>.Success(activity);
        }
    }
}