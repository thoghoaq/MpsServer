using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class GetOrders
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int ShopId { get; set; }
            public int? StatusId { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
        }

        public class Result
        {
            public List<Order> Orders { get; set; } = [];
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<GetOrders> logger, ILoggedUser loggedUser) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<GetOrders> _logger = logger;
            private readonly ILoggedUser _loggedUser = loggedUser;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!_loggedUser.IsShopOwnerOf(request.ShopId) && !_loggedUser.IsManagerGroup)
                    {
                        return CommandResult<Result>.Fail(_localizer["Unauthorized"]);
                    }

                    var query = _context.Orders
                        .Include(o => o.OrderDetails)
                        .Include(o => o.OrderStatus)
                        .Include(o => o.Progresses)
                        .Include(o => o.Shop)
                        .Include(o => o.PaymentMethod)
                        .Include(o => o.PaymentStatus)
                        .Where(o => o.ShopId == request.ShopId)
                        .Where(o => request.StatusId == null || o.OrderStatusId == request.StatusId)
                        .Where(s => request.Filter == null
                                   || (s.CustomerName != null && s.CustomerName.Contains(request.Filter))
                                   || (s.PhoneNumber != null && s.PhoneNumber.Contains(request.Filter))
                                   || (s.Address != null && s.Address.Contains(request.Filter))
                        )
                        .AsQueryable();

                    if (request.PageNumber.HasValue && request.PageSize.HasValue)
                    {
                        query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                    }
                    var orders = await query
                        .OrderByDescending(s => s.OrderDate)
                        .ToListAsync(cancellationToken: cancellationToken);

                    return CommandResult<Result>.Success(new Result { Orders = orders });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(_localizer["An error occurred while getting orders"]);
                }
            }
        }
    }
}
