using MediatR;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Seller
{
    public class GetShopDetail
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int Id { get; set; }
        }

        public class Result
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
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class Handler(MpsDbContext context, ILoggedUser loggedUser, IAppLocalizer localizer) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                var shop = _context.Shops.Where(s => s.Id == request.Id && s.ShopOwnerId == _loggedUser.UserId).FirstOrDefault();
                if (shop == null)
                {
                    return CommandResult<Result>.Fail(_localizer["Shop not found"]);
                }
                return CommandResult<Result>.Success(new Result
                {
                    Id = shop.Id,
                    ShopOwnerId = shop.ShopOwnerId,
                    ShopName = shop.ShopName,
                    PhoneNumber = shop.PhoneNumber,
                    Address = shop.Address,
                    City = shop.City,
                    District = shop.District,
                    Latitude = shop.Latitude,
                    Longitude = shop.Longitude,
                    Description = shop.Description,
                    Avatar = shop.Avatar,
                    Cover = shop.Cover,
                    IsActive = shop.IsActive,
                    CreatedAt = shop.CreatedAt,
                    UpdatedAt = shop.UpdatedAt,
                });
            }
        }
    }
}
