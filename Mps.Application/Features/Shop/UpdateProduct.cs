using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class UpdateProduct
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int ShopId { get; set; }
            public required int Id { get; set; }
            public string? Name { get; set; }
            public decimal? Price { get; set; }
            public int? Stock { get; set; }
            public string? Description { get; set; }
            public int? CategoryId { get; set; }
            public int? BrandId { get; set; }
            public List<Image>? Images { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public record Image
        {
            public int Id { get; set; }
            public required string ImagePath { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<UpdateProduct> logger, ILoggedUser loggedUser) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<UpdateProduct> _logger = logger;
            private readonly ILoggedUser _loggedUser = loggedUser;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (!_loggedUser.IsShopOwnerOf(request.ShopId))
                    {
                        return CommandResult<Result>.Fail(_localizer["Unauthorized"]);
                    }

                    var product = await _context.Products
                        .Include(p => p.Images)
                        .FirstOrDefaultAsync(e => e.Id == request.Id);
                    if (product == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["Product not found"]);
                    }

                    if (request.Name != null)
                    {
                        product.Name = request.Name;
                    }

                    if (request.Description != null)
                    {
                        product.Description = request.Description;
                    }

                    if (request.Price != null)
                    {
                        product.Price = request.Price.Value;
                    }

                    if (request.Stock != null)
                    {
                        product.Stock = request.Stock.Value;
                    }

                    if (request.CategoryId != null)
                    {
                        product.CategoryId = request.CategoryId.Value;
                    }

                    if (request.BrandId != null)
                    {
                        product.ModelId = request.BrandId.Value;
                    }

                    if (request.Images != null)
                    {
                        var images = request.Images.Select(i => new ProductImage { Id = i.Id, ImagePath = i.ImagePath }).ToList();
                        product.Images = images;
                    }

                    product.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    return CommandResult<Result>.Success(new Result { Message = _localizer["Product updated successfully"] });
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
