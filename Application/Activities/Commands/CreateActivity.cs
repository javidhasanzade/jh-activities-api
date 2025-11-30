using Application.Activities.DTOs;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class CreateActivity
{
    public class Command : IRequest<Result<string>>
    {
        public required CreateActivityDto ActivityDto { get; init; }
    }

    public class Handler(AppDbContext dbContext, IMapper mapper) : IRequestHandler<Command, Result<string>>
    {
        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = mapper.Map<Activity>(request.ActivityDto);

            dbContext.Activities.Add(activity);

            var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

            return !result
                ? Result<string>.Failure("Failed to create activity", 400)
                : Result<string>.Success(activity.Id);
        }
    }
}