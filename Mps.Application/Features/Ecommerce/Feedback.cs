using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Ecommerce
{
    public class Feedback
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public int ProductId { get; set; }
            public int OrderId { get; set; }
            public string? Feedback { get; set; }
            public int Rating { get; set; }
        }

        public class Result
        {
            public required string Message { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<Feedback> logger, ILoggedUser loggedUser) : IRequestHandler<Command, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.Rating < 1 || request.Rating > 5)
                    {
                        return CommandResult<Result>.Fail(localizer["Rating must be between 1 and 5"]);
                    }

                    var exist = await context.ProductFeedbacks.AnyAsync(f => f.ProductId == request.ProductId && f.UserId == loggedUser.UserId && request.OrderId == f.OrderId, cancellationToken);
                    if (exist)
                    {
                        return CommandResult<Result>.Fail(localizer["You have already sent feedback"]);
                    }

                    var feedback = new ProductFeedback
                    {
                        ProductId = request.ProductId,
                        OrderId = request.OrderId,
                        UserId = loggedUser.UserId,
                        Feedback = request.Feedback,
                        Rating = request.Rating,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.ProductFeedbacks.Add(feedback);

                    var orderDetail = await context.OrderDetails.FirstOrDefaultAsync(d => d.ProductId == request.ProductId && d.OrderId == request.OrderId, cancellationToken);
                    if (orderDetail == null)
                    {
                        return CommandResult<Result>.Fail(localizer["Order detail not found"]);
                    }
                    orderDetail.IsFeedbacked = true;

                    await context.SaveChangesAsync(cancellationToken);

                    return CommandResult<Result>.Success(new Result { Message = localizer["Feedback has been sent"] });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(localizer["Failed to send feedback"]);
                }
            }
        }
    }
}
