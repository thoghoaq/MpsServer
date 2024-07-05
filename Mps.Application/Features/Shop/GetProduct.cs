using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class GetProduct
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public required int Id { get; set; }
            public required int ShopId { get; set; }
        }

        public class Result
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public required decimal Price { get; set; }
            public required int Stock { get; set; }
            public string? Description { get; set; }
            public int CategoryId { get; set; }
            public int? ModelId { get; set; }
            public required int ShopId { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public List<Image>? Images { get; set; }
        }

        public record Image
        {
            public int Id { get; set; }
            public required string ImagePath { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<GetProduct> logger, ILoggedUser loggedUser) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<GetProduct> _logger = logger;
            private readonly ILoggedUser _loggedUser = loggedUser;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!_loggedUser.IsShopOwnerOf(request.ShopId))
                    {
                        return CommandResult<Result>.Fail(_localizer["Unauthorized"]);
                    }

                    var product = await _context.Products
                        .Include(p => p.Images)
                        .FirstOrDefaultAsync(p => p.Id == request.Id);
                    if (product == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["Product not found"]);
                    }

                    return CommandResult<Result>.Success(new Result
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Stock = product.Stock,
                        ModelId = product.ModelId,
                        CategoryId = product.CategoryId,
                        ShopId = product.ShopId,
                        IsActive = product.IsActive,
                        CreatedAt = product.CreatedAt,
                        UpdatedAt = product.UpdatedAt,
                        Images = product.Images.Select(i => new Image { Id = i.Id, ImagePath = i.ImagePath }).ToList()
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(_localizer["An error occurred while getting product"]);
                }
            }
        }
    }
}
