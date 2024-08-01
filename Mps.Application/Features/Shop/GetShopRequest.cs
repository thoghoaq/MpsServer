using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class GetShopRequest
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
        }

        public class Result
        {
            public List<ShopResult> Shops { get; set; } = [];
        }

        public class ShopResult : Domain.Entities.Shop
        {
            public ShopOwner? ShopOwner { get; set; }
        }

        public class ShopOwnerResult : ShopOwner
        {
            public User? User { get; set; }
        }

        public class QueryHandler(MpsDbContext context, IAppLocalizer localizer, ILogger<GetShopRequest> logger) : IRequestHandler<Query, CommandResult<Result>>
        {
            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var query = context.Shops
                        .Join(context.ShopOwners.Join(context.Users, u => u.UserId, x => x.Id, (u, x) => new { u, x }).Select(
                                y => new ShopOwnerResult
                                {
                                    UserId = y.u.UserId,
                                    User = y.x,
                                    CreatedAt = y.u.CreatedAt,
                                    UpdatedAt = y.u.UpdatedAt,
                                }
                            ), s => s.ShopOwnerId, so => so.UserId, (s, so) => new { s, so })
                        .Select(s => new ShopResult
                        {
                            Address = s.s.Address,
                            CreatedAt = s.s.CreatedAt,
                            Id = s.s.Id,
                            IsActive = s.s.IsActive,
                            PhoneNumber = s.s.PhoneNumber,
                            ShopName = s.s.ShopName,
                            City = s.s.City,
                            District = s.s.District,
                            IsAccepted = s.s.IsAccepted,
                            ShopOwner = s.so,
                            Avatar = s.s.Avatar,
                            Cover = s.s.Cover,
                            ShopOwnerId = s.s.ShopOwnerId,
                            UpdatedAt = s.s.UpdatedAt,
                            Description = s.s.Description,
                            Latitude = s.s.Latitude,
                            Longitude = s.s.Longitude,
                            PayPalAccount = s.s.PayPalAccount,
                        })
                        .Where(s => !s.IsActive)
                        .Where(s => request.Filter == null || s.ShopName.Contains(request.Filter));

                    if (request.PageNumber.HasValue && request.PageSize.HasValue)
                    {
                        query = query.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);
                    }
                    return CommandResult<Result>.Success(new Result { Shops = await query.OrderBy(e => e.CreatedAt).ToListAsync(cancellationToken) });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
