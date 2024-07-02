using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class GetShops
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public DateTime? MonthToDate { get; set; }
        }

        public class Result
        {
            public required List<Shop> Shops { get; set; }
        }

        public record Shop
        {
            public int Id { get; set; }
            public int ShopOwnerId { get; set; }
            public required string ShopName { get; set; }
            public required string PhoneNumber { get; set; }
            public required string Address { get; set; }
            public string? City { get; set; }
            public string? District { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public string? Description { get; set; }
            public string? Avatar { get; set; }
            public string? Cover { get; set; }
            public bool IsActive { get; set; }

            public string? PayPalAccount { get; set; }

            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            public bool IsCurrentMonthPaid { get; set; }
            public decimal? Revenue { get; set; }
            public decimal? ExpectPayout { get; set; }
            public decimal? TotalPayout { get; set; }
            public List<Payout> Payouts { get; set; } = [];
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<GetShops> logger) : IRequestHandler<Query, CommandResult<Result>>
        {

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var currentMonth = request.MonthToDate ?? DateTime.UtcNow;
                    var revenueQuery = context.Orders
                        .Where(o => request.MonthToDate == null || (o.OrderDate.Month == request.MonthToDate.Value.Month && o.OrderDate.Year == request.MonthToDate.Value.Year))
                        .Where(o => o.OrderStatusId == (int)Domain.Enums.OrderStatus.Completed)
                        .GroupBy(o => o.ShopId)
                        .Select(g => new
                        {
                            ShopId = g.Key,
                            Revenue = g.Sum(o => o.TotalAmount)
                        });

                    var shops = await context.Shops
                        .Where(s => s.IsActive)
                        .Include(s => s.Payouts)
                        .GroupJoin(revenueQuery, s => s.Id, r => r.ShopId, (s, r) => new { s, r })
                        .SelectMany(s => s.r.DefaultIfEmpty(), (s, r) => new { s.s, r })
                        .Select(s => new Shop
                        {
                            Id = s.s.Id,
                            ShopOwnerId = s.s.ShopOwnerId,
                            ShopName = s.s.ShopName,
                            PhoneNumber = s.s.PhoneNumber,
                            Address = s.s.Address,
                            City = s.s.City,
                            District = s.s.District,
                            Latitude = s.s.Latitude,
                            Longitude = s.s.Longitude,
                            Description = s.s.Description,
                            Avatar = s.s.Avatar,
                            Cover = s.s.Cover,
                            IsActive = s.s.IsActive,
                            PayPalAccount = s.s.PayPalAccount,
                            CreatedAt = s.s.CreatedAt,
                            UpdatedAt = s.s.UpdatedAt,
                            IsCurrentMonthPaid = s.s.Payouts.Any(p => p.MonthToDate.Month == currentMonth.Month && p.MonthToDate.Year == currentMonth.Year && p.PayoutStatusId == (int)Domain.Enums.PayoutStatus.Success),
                            Payouts = s.s.Payouts.OrderByDescending(p => p.MonthToDate).ToList(),
                            Revenue = s.r!.Revenue,
                            ExpectPayout = s.s.Payouts.FirstOrDefault(p => p.MonthToDate.Month == currentMonth.Month && p.MonthToDate.Year == currentMonth.Year)!.ExpectAmount,
                            TotalPayout = s.s.Payouts
                                .Where(o => request.MonthToDate == null || (o.MonthToDate.Month == request.MonthToDate.Value.Month && o.MonthToDate.Year == request.MonthToDate.Value.Year))
                                .Sum(p => p.Amount)
                        })
                        .OrderBy(s => s.ShopName)
                        .ToListAsync(cancellationToken);

                    return CommandResult<Result>.Success(new Result { Shops = shops });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(localizer["AnErrorOccurred"]);
                }
            }
        }
    }
}
