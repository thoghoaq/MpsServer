using MediatR;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Staff
{
    public class GetShops
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public string? Filter { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
        }

        public class Result
        {
            public List<ShopResult> Shops { get; set; } = [];
        }

        public class ShopResult : Domain.Entities.Shop
        {
            public ShopOwnerResult? ShopOwner { get; set; }
        }

        public class ShopOwnerResult : ShopOwner
        {
            public User? User { get; set; }
        }

        public class Handler(MpsDbContext context) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var shops = _context.Shops
                        .Join(_context.ShopOwners.Join(_context.Users, u => u.UserId, x => x.Id, (u, x) => new { u, x }).Select(
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
                            Comment = s.s.Comment,
                        })
                        .Where(s => s.IsActive)
                        .AsEnumerable()
                        .Where(s => request.Filter == null
                            || s.ShopName.SearchIgnoreCase(request.Filter)
                            || s.PhoneNumber.SearchIgnoreCase(request.Filter)
                            || s.Address.SearchIgnoreCase(request.Filter)
                        )
                        .OrderByDescending(s => s.CreatedAt)
                        .ToList();

                    if (request.PageNumber.HasValue && request.PageSize.HasValue)
                    {
                        shops = shops.Skip((request.PageNumber.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value).ToList();
                    }

                    return CommandResult<Result>.Success(new Result { Shops = shops });
                }
                catch (Exception ex)
                {
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
