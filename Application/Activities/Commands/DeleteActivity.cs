using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class DeleteActivity
{
    public class Command : IRequest
    {
        public required string Id { get; set; }
    }
    
    public class Handler(AppDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await dbContext.Activities
                               .FindAsync([request.Id], cancellationToken)
                           ?? throw new Exception("Activity not found");
            
            dbContext.Activities.Remove(activity);
            
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}