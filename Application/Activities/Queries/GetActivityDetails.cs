using Application.Activities.DTOs;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    public class Query : IRequest<Result<ActivityDto>>
    {
        public required string Id { get; set; }
    }

    public class Handler(AppDbContext dbContext, IMapper mapper) : IRequestHandler<Query, Result<ActivityDto>>
    {
        public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var activity = await dbContext.Activities.Include(x => x.Attendees)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return activity == null
                ? Result<ActivityDto>.Failure("Activity not found", 404)
                : Result<ActivityDto>.Success(mapper.Map<ActivityDto>(activity));
        }
    }
}