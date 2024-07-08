using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Ecommerce
{
    public class GetFeedbacks
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int ProductId { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
        }

        public class Result
        {
            public double AverageRating { get; set; }
            public int Total { get; set; }
            public int FiveStar { get; set; }
            public int FourStar { get; set; }
            public int ThreeStar { get; set; }
            public int TwoStar { get; set; }
            public int OneStar { get; set; }
            public required List<ProductFeedback> Feedbacks { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<GetFeedbacks> logger) : IRequestHandler<Query, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var query = context.ProductFeedbacks
                        .Where(f => f.ProductId == request.ProductId)
                        .AsQueryable();

                    if (request.PageNumber.HasValue && request.PageSize.HasValue)
                    {
                        query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                    }
                    var feedbacks = await query
                        .OrderByDescending(f => f.CreatedAt)
                        .ToListAsync(cancellationToken: cancellationToken);

                    var result = new Result
                    {
                        AverageRating = feedbacks.Count > 0 ? feedbacks.Average(f => f.Rating) : 0,
                        Total = feedbacks.Count,
                        FiveStar = feedbacks.Count(f => f.Rating == 5),
                        FourStar = feedbacks.Count(f => f.Rating == 4),
                        ThreeStar = feedbacks.Count(f => f.Rating == 3),
                        TwoStar = feedbacks.Count(f => f.Rating == 2),
                        OneStar = feedbacks.Count(f => f.Rating == 1),
                        Feedbacks = feedbacks
                    };

                    return CommandResult<Result>.Success(result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(localizer["Failed to get feedbacks"]);
                }
            }
        }
    }
}
