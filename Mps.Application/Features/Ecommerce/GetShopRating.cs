using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Ecommerce
{
    public class GetShopRating
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int ShopId { get; set; }
        }

        public class Result
        {
            public required double Rating { get; set; }
        }

        public class Handler(MpsDbContext context, ILogger<GetShopRating> logger) : IRequestHandler<Query, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var rating = await context.Products
                        .Include(p => p.Feedbacks)
                        .Where(p => p.ShopId == request.ShopId)
                        .AverageAsync(p => p.Feedbacks.Average(f => f.Rating), cancellationToken);

                    return CommandResult<Result>.Success(new Result { Rating = Math.Round(rating, 1) });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "GetShopRatingFailure");
                    return CommandResult<Result>.Fail("Error getting shop rating");
                }
            }
        }
    }
}
