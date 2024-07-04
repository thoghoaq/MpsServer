using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Enums;

namespace Mps.Application.Features.Ecommerce
{
    public class TrackingProduct
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public List<int> ProductId { get; set; } = [];
            public TrackingProductAction Action { get; set; }
        }

        public class Result
        {
            public required bool Success { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILogger<TrackingProduct> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var products = await dbContext.Products
                        .Where(p => request.ProductId.Contains(p.Id))
                        .ToListAsync(cancellationToken);

                    if (products.Count == 0)
                    {
                        return CommandResult<Result>.Fail("Product not found");
                    }

                    foreach (var product in products)
                    {
                        switch (request.Action)
                        {
                            case TrackingProductAction.View:
                                product.ViewCount++;
                                break;
                            case TrackingProductAction.Buy:
                                product.SoldCount++;
                                break;
                        }
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);

                    return CommandResult<Result>.Success(new Result { Success = true });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error tracking product");
                    return CommandResult<Result>.Fail("Error tracking product");
                }
            }
        }
    }
}
