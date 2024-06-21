using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class CreateProduct
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int ShopId { get; set; }
            public required string Name { get; set; }
            public required decimal Price { get; set; }
            public required int Stock { get; set; }
            public required string ProductCode { get; set; }
            public required string ProductSKU { get; set; }
            public string? Description { get; set; }
            public int CategoryId { get; set; }
            public int? BrandId { get; set; }
            public List<Image>? Images { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public record Image
        {
            public required string ImagePath { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<CreateProduct> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<CreateProduct> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var shop = await _context.Shops.FindAsync(request.ShopId);
                    if (shop == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["Shop not found"]);
                    }

                    var product = new Product
                    {
                        ShopId = request.ShopId,
                        Name = request.Name,
                        Description = request.Description,
                        Price = request.Price,
                        Stock = request.Stock,
                        BrandId = request.BrandId,
                        CategoryId = request.CategoryId,
                        Images = request.Images?.Select(i => new ProductImage { ImagePath = i.ImagePath }).ToList() ?? new List<ProductImage>(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Product created successfully"] });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateProductFailure");
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }
        }
    }
}
