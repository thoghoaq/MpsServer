using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Payment
{
    public class GetPaymentDetails
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int PaymentId { get; set; }
        }

        public class Result
        {
            public List<Domain.Entities.Order> Orders { get; set; } = [];
        }

        public class Handler(MpsDbContext dbContext) : IRequestHandler<Query, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = new Result();
                var paymentRefs = dbContext.PaymentRef
                    .Where(p => p.PaymentId == request.PaymentId)
                    .Select(p => p.RefId)
                    .ToList();
                if (paymentRefs.Count == 0)
                {
                    return CommandResult<Result>.Fail("Payment not found");
                }

                result.Orders = dbContext.Orders.Where(o => paymentRefs.Contains(o.Id))
                    .Include(o => o.OrderDetails)
                    .Include(o => o.OrderStatus)
                    .Include(o => o.Progresses)
                    .Include(o => o.Shop)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.PaymentStatus)
                    .ToList();
                return CommandResult<Result>.Success(result);
            }
        }
    }
}
