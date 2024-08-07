using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Enums;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Shop
{
    public class GetOrdersInPayoutDate
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int ShopId { get; set; }
            public DateTime MonthToDate { get; set; }
            public PayoutDate PayoutDate { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
        }

        public class Result
        {
            public List<Domain.Entities.Order> Orders { get; set; } = [];
        }

        public class Handler : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context;

            public Handler(MpsDbContext context)
            {
                _context = context;
            }

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var currentMonth = request.MonthToDate;
                if (request.PayoutDate == PayoutDate.Day1)
                {
                    currentMonth = request.MonthToDate.AddMonths(1);
                }
                var query = _context.Orders
                    .Include(o => o.OrderDetails)
                    .Include(o => o.OrderStatus)
                    .Include(o => o.Progresses)
                    .Include(o => o.Shop)
                    .Include(o => o.PaymentMethod)
                    .Include(o => o.PaymentStatus)
                    .Where(o => o.ShopId == request.ShopId && o.OrderStatusId == (int)Domain.Enums.OrderStatus.Completed)
                    .AsEnumerable()
                    .Where(o => o.OrderDate.InPayoutDate(currentMonth, request.PayoutDate));

                if (request.PageNumber.HasValue && request.PageSize.HasValue)
                {
                    query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                }

                return CommandResult<Result>.Success(new Result
                {
                    Orders = query.ToList()
                });
            }
        }
    }
}
