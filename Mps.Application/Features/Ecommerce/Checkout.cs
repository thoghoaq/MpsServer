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
            public int ShopId { get; set; }
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
            public int OrderId { get; set; }
            public int PaymentId { get; set; }
            public string? PaymentUrl { get; set; }
        }

        public class CheckoutItem
        {
            public int ProductId { get; set; }
            public required string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal? Discount { get; set; }
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
                    var order = new Order
                    {
                        CustomerId = _loggedUser.UserId,
                        ShopId = request.ShopId,
                        CustomerName = request.CustomerName,
                        Address = request.Address,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        Note = request.Note,
                        Discount = request.Discount,
                        TotalAmount = request.Items.Sum(x => x.Price * x.Quantity - (x.Discount ?? 0)) - (request.Discount ?? 0),
                        PaymentMethodId = (int)request.PaymentMethod,
                        OrderDetails = request.Items.Select(x => new OrderDetail
                        {
                            ProductId = x.ProductId,
                            ProductName = x.ProductName,
                            Quantity = x.Quantity,
                            Price = x.Price,
                            Discount = x.Discount ?? 0,
                            Total = x.Price * x.Quantity - (x.Discount ?? 0)
                        }).ToList(),
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
                    await _context.Orders.AddAsync(order, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    var paymentResult = await _mediator.Send(new CreatePayment.Command
                    {
                        MerchantId = request.ShopId,
                        PaymentContent = $"Thanh toán đơn hàng {order.Id}",
                        PaymentCurrency = "VND",
                        PaymentDestinationId = request.PaymentMethod.GetDescription(),
                        PaymentLanguage = "vn",
                        PaymentRefId = order.Id,
                        RequiredAmount = order.TotalAmount
                    }, cancellationToken);
                    if (!paymentResult.IsSuccess)
                    {
                        return CommandResult<Result>.Fail(paymentResult.FailureReason!);
                    }
                    return CommandResult<Result>.Success(new Result
                    {
                        OrderId = order.Id,
                        PaymentId = paymentResult.Payload!.PaymentId,
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
