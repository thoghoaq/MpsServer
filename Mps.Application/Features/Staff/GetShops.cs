using MediatR;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Staff
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
            public List<Domain.Entities.Shop> Shops { get; set; } = [];
        }

        public class Handler(MpsDbContext context) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var shops = _context.Shops
                        .Where(s => s.IsActive)
                        .AsEnumerable()
                        .Where(s => request.Filter == null
                            || s.ShopName.SearchIgnoreCase(request.Filter)
                            || s.PhoneNumber.SearchIgnoreCase(request.Filter)
                            || s.Address.SearchIgnoreCase(request.Filter)
                        )
                        .OrderByDescending(s => s.CreatedAt)
                        .ToList();

                    if (request.PageNumber.HasValue && request.PageSize.HasValue)
                    {
                        shops = shops.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value).ToList();
                    }

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
