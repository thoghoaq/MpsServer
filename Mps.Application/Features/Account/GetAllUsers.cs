using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Account
{
    public class GetAllUsers
    {
        public class Query : IRequest<CommandResult<Result>>
        {
        }

        public class  Result
        {
            public required List<User> Users { get; set; }
        }

        public class QueryHandler(MpsDbContext context) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var users = await _context.Users.ToListAsync(cancellationToken: cancellationToken);
                return CommandResult<Result>.Success(new Result { Users = users });
            }
        }
    }
}
