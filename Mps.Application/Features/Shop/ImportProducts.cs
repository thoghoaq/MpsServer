using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Excel;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Shop
{
    public class ImportProducts
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required int ShopId { get; set; }
            public required IFormFile File { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
            public List<ExecuteResult>? Results { get; set; }
        }

        public record ExecuteResult
        {
            public int Row { get; set; }
            public int? Id { get; set; }
            public string? Name { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public string? Description { get; set; }
            public int CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public int? ModelId { get; set; }
            public string? ModelName { get; set; }
            public bool IsSuccess => string.IsNullOrEmpty(Message);
            public string? Message { get; set; }
        }

        public record Product
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public string? Description { get; set; }
            public int CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public int? ModelId { get; set; }
            public string? ModelName { get; set; }
        }

        public class Handler(IExcelService excelService, MpsDbContext context, ILogger<ImportProducts> logger, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly IExcelService _excelService = excelService;
            private readonly MpsDbContext _context = context;
            private readonly ILogger<ImportProducts> _logger = logger;
            private readonly IAppLocalizer _localizer = localizer;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.File == null || request.File.Length == 0)
                    {
                        return CommandResult<Result>.Fail(_localizer["File is empty"]);
                    }

                    using var stream = new MemoryStream();
                    await request.File.CopyToAsync(stream);
                    stream.Position = 0;
                    var importedProducts = _excelService.ReadExcelFile<Product>(stream);
                    if (importedProducts == null || importedProducts.Count == 0)
                    {
                        return CommandResult<Result>.Fail(_localizer["No data found"]);
                    }

                    var results = new List<ExecuteResult>();
                    foreach (var product in importedProducts)
                    {
                        var result = new ExecuteResult
                        {
                            Name = product.Name,
                            Price = product.Price,
                            Stock = product.Stock,
                            Description = product.Description,
                            CategoryId = product.CategoryId,
                            ModelId = product.ModelId,
                            CategoryName = product.CategoryName,
                            ModelName = product.ModelName,
                            Row = importedProducts.IndexOf(product) + 1
                        };

                        if (string.IsNullOrEmpty(product.Name))
                        {
                            result.Message = _localizer["Name is required"];
                            results.Add(result);
                            continue;
                        }

                        if (product.Price <= 0)
                        {
                            result.Message = _localizer["Price must be greater than 0"];
                            results.Add(result);
                            continue;
                        }

                        if (product.Stock < 0)
                        {
                            result.Message = _localizer["Stock must be greater than or equal to 0"];
                            results.Add(result);
                            continue;
                        }

                        if (product.CategoryId <= 0)
                        {
                            result.Message = _localizer["Category is required"];
                            results.Add(result);
                            continue;
                        }

                        if (product.ModelId.HasValue && product.ModelId <= 0)
                        {
                            result.Message = _localizer["Model is invalid"];
                            results.Add(result);
                            continue;
                        }

                        var category = await _context.ProductCategories.FirstOrDefaultAsync(x => x.Id == product.CategoryId, cancellationToken);
                        if (category == null)
                        {
                            result.Message = _localizer["Category is invalid"];
                        }
                    }

                    var databaseProducts = await _context.Products.ToListAsync(cancellationToken);
                    var existingProducts = results.Where(c => c.Id != null && c.IsSuccess);
                    foreach (var product in existingProducts)
                    {
                        var existingProduct = databaseProducts.Find(x => x.Id == product.Id);
                        if (existingProduct == null)
                        {
                            product.Message = _localizer["Product not found"];
                            continue;
                        }

                        existingProduct.Name = product.Name!;
                        existingProduct.Price = product.Price;
                        existingProduct.Stock = product.Stock;
                        existingProduct.Description = product.Description;
                        existingProduct.CategoryId = product.CategoryId;
                        existingProduct.ModelId = product.ModelId;
                        _context.Products.Update(existingProduct);
                    }

                    var newProducts = results.Where(c => c.Id == null && c.IsSuccess);
                    foreach (var product in newProducts)
                    {
                        var newProduct = new Product
                        {
                            Name = product.Name!,
                            Price = product.Price,
                            Stock = product.Stock,
                            Description = product.Description,
                            CategoryId = product.CategoryId,
                            ModelId = product.ModelId
                        };
                        _context.Products.Add(new Domain.Entities.Product
                        {
                            Name = newProduct.Name,
                            Price = newProduct.Price,
                            Stock = newProduct.Stock,
                            Description = newProduct.Description,
                            CategoryId = newProduct.CategoryId,
                            ModelId = newProduct.ModelId,
                            ShopId = request.ShopId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsActive = false,
                        });
                    }
                    await _context.SaveChangesAsync(cancellationToken);

                    return CommandResult<Result>.Success(new Result
                    {
                        Message = _localizer["Import success"],
                        Results = results
                    });
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