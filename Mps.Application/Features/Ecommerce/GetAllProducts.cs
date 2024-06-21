using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Ecommerce
{
    public class GetAllProducts
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public List<int>? CategoriesId { get; set; }
            public List<int>? BrandsId { get; set; }
            public List<int>? ShopsId { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
        }

        public class Result
        {
            public required List<Product> Products { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<GetAllProducts> logger) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<GetAllProducts> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var query = _context.Products
                        .Include(p => p.Images)
                        .Include(p => p.Category)
                        .Include(p => p.Brand)
                        .Include(p => p.Shop)
                        .Where(p => p.IsActive)
                        .Where(s => request.Filter == null
                            || s.Name.SearchIgnoreCase(request.Filter)
                        )
                        .AsQueryable();

                    if (request.CategoriesId != null && request.CategoriesId.Any())
                    {
                        query = query.Where(p => request.CategoriesId.Contains(p.CategoryId));
                    }

                    if (request.BrandsId != null && request.BrandsId.Any())
                    {
                        query = query.Where(p => p.BrandId != null && request.BrandsId.Contains((int)p.BrandId));
                    }

                    if (request.ShopsId != null && request.ShopsId.Any())
                    {
                        query = query.Where(p => request.ShopsId.Contains(p.ShopId));
                    }

                    if (request.PageNumber.HasValue && request.PageSize.HasValue)
                    {
                        query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                    }
                    var products = await query
                        .OrderByDescending(s => s.UpdatedAt)
                        .ToListAsync(cancellationToken: cancellationToken);

                    return CommandResult<Result>.Success(new Result { Products = products });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetAllProductsFailure");
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }
        }
    }
}
