using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class DeactiveProduct
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int Id { get; set; }
            public required bool IsActive { get; set; } = false;
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<DeactiveProduct> logger, ILoggedUser loggedUser) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<DeactiveProduct> _logger = logger;
            private readonly ILoggedUser _loggedUser = loggedUser;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var product = await _context.Products.FindAsync(request.Id);
                    if (product == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["Product not found"]);
                    }
                    if (!_loggedUser.IsShopOwnerOf(product.ShopId))
                    {
                        return CommandResult<Result>.Fail(_localizer["Unauthorized"]);
                    }

                    product.IsActive = request.IsActive;
                    await _context.SaveChangesAsync(cancellationToken);

                    return CommandResult<Result>.Success(new Result { Message = _localizer[$"Product {(!request.IsActive ? "deactivated" : "activated")}"] });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when deactive product");
                    return CommandResult<Result>.Fail(_localizer["Error when deactive product"]);
                }
            }
        }
    }
}
