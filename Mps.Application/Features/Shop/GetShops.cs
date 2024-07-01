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
            public List<Payout> Payouts { get; set; } = [];
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<GetShops> logger) : IRequestHandler<Query, CommandResult<Result>>
        {

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var shops = await context.Shops
                        .Include(s => s.Payouts)
                        .Select(s => new Shop
                        {
                            Id = s.Id,
                            ShopOwnerId = s.ShopOwnerId,
                            ShopName = s.ShopName,
                            PhoneNumber = s.PhoneNumber,
                            Address = s.Address,
                            City = s.City,
                            District = s.District,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            Description = s.Description,
                            Avatar = s.Avatar,
                            Cover = s.Cover,
                            IsActive = s.IsActive,
                            PayPalAccount = s.PayPalAccount,
                            CreatedAt = s.CreatedAt,
                            UpdatedAt = s.UpdatedAt,
                            IsCurrentMonthPaid = s.Payouts.Any(p => p.MonthToDate.Month == DateTime.Now.Month && p.MonthToDate.Year == DateTime.Now.Year && p.PayoutStatusId == (int)Domain.Enums.PayoutStatus.Success),
                            Payouts = s.Payouts.OrderByDescending(p => p.MonthToDate).ToList()
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
