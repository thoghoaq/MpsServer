using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Application.Helpers;
using Mps.Domain.Entities;
using Mps.Domain.Enums;
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
            public ProductFilter? FilterBy { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
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
                        .AsEnumerable()
                        .Where(s => request.Filter == null
                            || s.Name.SearchIgnoreCase(request.Filter)
                        );

                    if (request.CategoriesId != null && request.CategoriesId.Any())
                    {
                        var categories = GetAllCategoriesId(request.CategoriesId);
                        query = query.Where(p => categories.Contains(p.CategoryId));
                    }

                    if (request.BrandsId != null && request.BrandsId.Any())
                    {
                        query = query.Where(p => p.BrandId != null && request.BrandsId.Contains((int)p.BrandId));
                    }

                    if (request.ShopsId != null && request.ShopsId.Any())
                    {
                        query = query.Where(p => request.ShopsId.Contains(p.ShopId));
                    }

                    if (request.FilterBy.HasValue)
                    {
                        switch (request.FilterBy.Value)
                        {
                            case ProductFilter.Newest:
                                query = query.OrderByDescending(s => s.CreatedAt);
                                break;
                            case ProductFilter.Popular:
                                query = query.OrderByDescending(s => s.ViewCount);
                                break;
                            case ProductFilter.BestSeller:
                                query = query.OrderByDescending(s => s.SoldCount);
                                break;
                            case ProductFilter.PriceLowToHigh:
                                query = query.OrderBy(s => s.Price);
                                break;
                            case ProductFilter.PriceHighToLow:
                                query = query.OrderByDescending(s => s.Price);
                                break;
                        }
                    }

                    if (request.Latitude != null && request.Longitude != null)
                    {
                        query = query.OrderBy(s => CalculateHelper.CalculateDistance(s.Shop!.Latitude, s.Shop.Longitude, (double)request.Latitude, (double)request.Longitude));
                    }

                    if (request.PageNumber.HasValue && request.PageSize.HasValue)
                    {
                        query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                    }
                    var products = query
                        .ToList();

                    return CommandResult<Result>.Success(new Result { Products = products });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetAllProductsFailure");
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }

            private List<int> GetAllCategoriesId(List<int> categoriesId)
            {
                return _context.ProductCategories
                    .Include(c => c.Children)
                    .Where(c => categoriesId.Contains(c.Id)
                        || (c.ParentId.HasValue && categoriesId.Contains(c.ParentId.Value))
                        || c.Children.Any(x => x.ParentId.HasValue && categoriesId.Contains(x.ParentId.Value))
                    )
                    .Select(x => x.Id).ToList();
            }
        }
    }
}
