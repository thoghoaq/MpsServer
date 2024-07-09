using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Ecommerce
{
    public class GetSimilarProducts
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int ProductId { get; set; }
        }

        public class Result
        {
            public required List<Product> Products { get; set; }
        }

        public class Handler(MpsDbContext context, ILogger<GetSimilarProducts> logger) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly ILogger<GetSimilarProducts> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var product = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Images)
                        .Include(p => p.Model)
                        .Include(p => p.Shop)
                        .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

                    if (product == null)
                    {
                        return CommandResult<Result>.Fail("Product not found");
                    }

                    var query = _context.Products
                        .Include(p => p.Images)
                        .Include(p => p.Category)
                            .ThenInclude(c => c.Children)
                                .ThenInclude(c => c.Children)
                        .Include(p => p.Model)
                        .Include(p => p.Shop)
                        .Where(p => p.CategoryId == product.CategoryId
                        || (p.Category != null && p.Category.Children.Select(x => x.Id).Contains(product.CategoryId))
                        || (p.Category != null && p.Category.Children.Any(x => x.Children.Select(y => y.Id).Contains(product.CategoryId)))
                        )
                        .Where(p => p.Id != product.Id)
                        .AsQueryable();

                    var products = await query
                        .OrderByDescending(p => p.ViewCount)
                        .ToListAsync(cancellationToken: cancellationToken);

                    return CommandResult<Result>.Success(new Result { Products = products });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting similar products");
                    return CommandResult<Result>.Fail("Error getting similar products");
                }
            }
        }
    }
}
