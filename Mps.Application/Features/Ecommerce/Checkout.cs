using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Application.Features.Payment;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Ecommerce
{
    public class Checkout
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required string CustomerName { get; set; }
            public required string Address { get; set; }
            public required string Email { get; set; }
            public required string PhoneNumber { get; set; }
            public string? Note { get; set; }
            public required List<CheckoutItem> Items { get; set; }
            public decimal? Discount { get; set; }
            public Domain.Enums.PaymentMethod PaymentMethod { get; set; }
        }

        public class Result
        {
            public List<int> OrderIds { get; set; }
            public string? PaymentUrl { get; set; }
        }

        public class CheckoutItem
        {
            public int ProductId { get; set; }
            public required string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal? Discount { get; set; }
            public int ShopId { get; set; }
        }

        public class Handler(MpsDbContext context, ILogger<Checkout> logger, ILoggedUser loggedUser, IAppLocalizer localizer, IMediator mediator) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly ILogger<Checkout> _logger = logger;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly IMediator _mediator = mediator;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var listOrders = new List<Order>();
                    foreach (var item in request.Items)
                    {
                        var order = new Order
                        {
                            CustomerId = _loggedUser.UserId,
                            ShopId = item.ShopId,
                            CustomerName = request.CustomerName,
                            Address = request.Address,
                            Email = request.Email,
                            PhoneNumber = request.PhoneNumber,
                            Note = request.Note,
                            Discount = request.Discount,
                            TotalAmount = item.Price * item.Quantity - (item.Discount ?? 0) - (request.Discount ?? 0),
                            PaymentMethodId = (int)request.PaymentMethod,
                            OrderDetails = new List<OrderDetail>
                            {
                                new OrderDetail
                                {
                                    ProductId = item.ProductId,
                                    ProductName = item.ProductName,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    Discount = item.Discount ?? 0,
                                    Total = item.Price * item.Quantity - (item.Discount ?? 0)
                                }
                            },
                            OrderDate = DateTime.UtcNow,
                            DeliveryDate = DateTime.UtcNow.AddDays(7),
                            OrderStatusId = (int)Domain.Enums.OrderStatus.Pending,
                            PaymentStatusId = (int)Domain.Enums.PaymentStatus.Pending,
                            Progresses =
                            [
                                new()
                                {
                                    Name = Domain.Enums.OrderStatus.Pending.GetDescription(),
                                    CreatedDate = DateTime.UtcNow,
                                    UpdatedDate = DateTime.UtcNow,
                                }
                            ]
                        };
                        listOrders.Add(order);
                    }

                    await _context.Orders.AddRangeAsync(listOrders, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    var paymentResult = await _mediator.Send(new CreatePayment.Command
                    {
                        PaymentContent = $"Thanh toán đơn hàng {string.Join(',', listOrders.Select(x => x.Id))}",
                        PaymentCurrency = "VND",
                        PaymentDestinationId = request.PaymentMethod.GetDescription(),
                        PaymentLanguage = "vn",
                        RequiredAmount = listOrders.Sum(x => x.TotalAmount),
                        Merchants = listOrders.Select(x => new CreatePayment.Merchant
                        {
                            PaymentRefId = x.Id,
                            MerchantId = x.ShopId
                        }).ToList()
                    }, cancellationToken);
                    if (!paymentResult.IsSuccess)
                    {
                        return CommandResult<Result>.Fail(paymentResult.FailureReason!);
                    }
                    return CommandResult<Result>.Success(new Result
                    {
                        OrderIds = listOrders.Select(x => x.Id).ToList(),
                        PaymentUrl = paymentResult.Payload?.PaymentUrl
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }
        }
    }
}
