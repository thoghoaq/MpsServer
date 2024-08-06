using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
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

        public class Handler(MpsDbContext context, ILogger<GetShopRating> logger, IAppLocalizer localizer) : IRequestHandler<Query, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var products = context.Products
                        .Include(p => p.Feedbacks)
                        .Where(p => p.ShopId == request.ShopId);

                    var feedbacks = products.SelectMany(p => p.Feedbacks);
                    if (!feedbacks.Any())
                    {
                        return CommandResult<Result>.Success(new Result { Rating = 0 });
                    }
                    var rating = feedbacks.Average(f => f.Rating);
                    return CommandResult<Result>.Success(new Result { Rating = Math.Round(rating, 1) });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "GetShopRatingFailure");
                    return CommandResult<Result>.Fail(localizer["Error getting shop rating"]);
                }
            }
        }
    }
}
