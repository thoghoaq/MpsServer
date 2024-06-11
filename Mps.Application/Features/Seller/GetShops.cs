using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Seller
{
    public class GetShops
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public string? Filter { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
        }

        public class Result
        {
            public required List<Domain.Entities.Shop> Shops { get; set; }
        }

        public class QueryHandler(MpsDbContext context, ILoggedUser loggedUser) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Shops
                    .Where(s => s.ShopOwnerId == loggedUser.UserId)
                    .Where(s => request.Filter == null
                        || s.ShopName.Contains(request.Filter)
                     )
                    .AsQueryable();
                if (request.PageNumber.HasValue && request.PageSize.HasValue)
                {
                    query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                }
                var shops = await query
                    .OrderByDescending(s => s.UpdatedAt)
                    .ToListAsync(cancellationToken: cancellationToken);
                return CommandResult<Result>.Success(new Result { Shops = shops });
            }
        }
    }
}
