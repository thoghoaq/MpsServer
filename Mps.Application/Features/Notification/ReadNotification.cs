using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Notification
{
    public class ReadNotification
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public List<int> NotificationId { get; set; } = [];
        }

        public class Result
        {
            public bool Success { get; set; }
        }

        public class Handler : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context;

            public Handler(MpsDbContext context)
            {
                _context = context;
            }

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                var notifications = await _context.Notifications.Where(n => request.NotificationId.Contains(n.Id)).ToListAsync(cancellationToken);
                notifications.ForEach(n => n.ReadAt = DateTime.UtcNow);
                await _context.SaveChangesAsync(cancellationToken);

                return CommandResult<Result>.Success(new Result { Success = true });
            }
        }
    }
}
