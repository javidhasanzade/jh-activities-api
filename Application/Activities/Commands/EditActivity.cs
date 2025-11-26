using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class EditActivity
{
    public class Command : IRequest
    {
        public required Activity Activity { get; set; }
    }

    public class Handler(AppDbContext dbContext, IMapper mapper) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await dbContext.Activities
                               .FindAsync([request.Activity.Id], cancellationToken)
                           ?? throw new Exception("Activity not found");

            mapper.Map(request.Activity, activity);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}