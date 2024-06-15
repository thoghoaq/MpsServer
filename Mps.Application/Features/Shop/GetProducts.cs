using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class GetProducts
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int ShopId { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
        }

        public class Result
        {
            public required List<Product> Products { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<GetProducts> logger, ILoggedUser loggedUser) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<GetProducts> _logger = logger;
            private readonly ILoggedUser _loggedUser = loggedUser;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!_loggedUser.IsShopOwnerOf(request.ShopId))
                    {
                        return CommandResult<Result>.Fail(_localizer["Unauthorized"]);
                    }

                    var query = _context.Products
                        .Include(p => p.Images)
                        .Include(p => p.Category)
                        .Include(p => p.Brand)
                        .Where(p => p.ShopId == request.ShopId)
                        .Where(s => request.Filter == null
                            || s.Name.Contains(request.Filter)
                        )
                        .AsQueryable();

                    if (request.PageNumber.HasValue && request.PageSize.HasValue)
                    {
                        query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                    }
                    var products = await query
                        .OrderBy(s => s.Name)
                        .ToListAsync(cancellationToken: cancellationToken);

                    return CommandResult<Result>.Success(new Result { Products = products });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetProductsFailure");
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }
        }
    }
}
