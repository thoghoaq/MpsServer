using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Seller
{
    public class UpdateShop
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int Id { get; set; }
            public string? ShopName { get; set; }
            public string? Address { get; set; }
            public string? PhoneNumber { get; set; }
            public string? PayPalAccount { get; set; }
            public string? City { get; set; }
            public string? District { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public string? Description { get; set; }
            public string? Avatar { get; set; }
            public string? Cover { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext context, ILoggedUser loggedUser, IAppLocalizer localizer, ILogger<UpdateShop> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<UpdateShop> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var shop = await _context.Shops.FirstOrDefaultAsync(e => e.Id == request.Id);
                    if (shop == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["Shop not found"]);
                    }

                    if (!_loggedUser.IsShopOwnerOf(shop.Id))
                    {
                        return CommandResult<Result>.Fail(_localizer["Unauthorized"]);
                    }

                    if (request.ShopName != null)
                    {
                        shop.ShopName = request.ShopName;
                    }

                    if (request.Address != null)
                    {
                        shop.Address = request.Address;
                    }

                    if (request.PhoneNumber != null)
                    {
                        shop.PhoneNumber = request.PhoneNumber;
                    }

                    if (request.PayPalAccount != null)
                    {
                        shop.PayPalAccount = request.PayPalAccount;
                    }

                    if (request.City != null)
                    {
                        shop.City = request.City;
                    }

                    if (request.District != null)
                    {
                        shop.District = request.District;
                    }

                    if (request.Latitude != null)
                    {
                        shop.Latitude = request.Latitude;
                    }

                    if (request.Longitude != null)
                    {
                        shop.Longitude = request.Longitude;
                    }

                    if (request.Description != null)
                    {
                        shop.Description = request.Description;
                    }

                    if (request.Avatar != null)
                    {
                        shop.Avatar = request.Avatar;
                    }

                    if (request.Cover != null)
                    {
                        shop.Cover = request.Cover;
                    }

                    shop.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Shop updated successfully"] });
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
