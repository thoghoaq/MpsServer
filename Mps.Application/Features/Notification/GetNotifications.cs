using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Notification
{
    public class GetNotifications
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int UserId { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
        }

        public class Result
        {
            public List<UserNotification> Notifications { get; set; } = [];
        }

        public class Handler : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context;

            public Handler(MpsDbContext context)
            {
                _context = context;
            }

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Notifications
                    .Where(n => n.UserId == request.UserId)
                    .OrderByDescending(n => n.CreatedAt)
                    .AsQueryable();

                if (request.PageNumber.HasValue && request.PageSize.HasValue)
                {
                    query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                }

                var notifications = await query.ToListAsync(cancellationToken: cancellationToken);

                return CommandResult<Result>.Success(new Result { Notifications = notifications });
            }
        }
    }
}
