using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Staff
{
    public class GetShops
    {
        public class Query : IRequest<CommandResult<Result>>
        {
        }

        public class Result
        {
            public List<Domain.Entities.Shop> Shops { get; set; } = [];
        }

        public class Handler(MpsDbContext context) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var shops = await _context.Shops
                        .OrderBy(s => s.IsActive)
                        .ToListAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Shops = shops });
                }
                catch (Exception ex)
                {
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
