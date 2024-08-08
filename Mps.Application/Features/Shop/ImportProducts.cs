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
            public string? Price { get; set; }
            public string? Stock { get; set; }
            public string? Description { get; set; }
            public string? CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public bool IsSuccess => string.IsNullOrEmpty(Message);
            public string? Message { get; set; }
        }

        public record Product
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? Price { get; set; }
            public string? Stock { get; set; }
            public string? Description { get; set; }
            public string? CategoryId { get; set; }
            public string? CategoryName { get; set; }
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
                    var categories = _context.ProductCategories.ToList();

                    var results = new List<ExecuteResult>();
                    foreach (var product in importedProducts)
                    {
                        var result = new ExecuteResult
                        {
                            Id = !string.IsNullOrEmpty(product.Id) ? int.Parse(product.Id) : null,
                            Name = product.Name,
                            Price = product.Price,
                            Stock = product.Stock,
                            Description = product.Description,
                            CategoryId = product.CategoryId,
                            CategoryName = product.CategoryName,
                            Row = importedProducts.IndexOf(product) + 1
                        };

                        if (string.IsNullOrEmpty(product.Name))
                        {
                            result.Message = _localizer["Name is required"];
                            results.Add(result);
                            continue;
                        }

                        decimal.TryParse(product.Price, out var price);
                        if (price <= 0)
                        {
                            result.Message = _localizer["Price must be greater than 0"];
                            results.Add(result);
                            continue;
                        }

                        int.TryParse(product.Stock, out var stock);
                        if (stock < 0)
                        {
                            result.Message = _localizer["Stock must be greater than or equal to 0"];
                            results.Add(result);
                            continue;
                        }

                        if (string.IsNullOrEmpty(product.CategoryId))
                        {
                            result.Message = _localizer["Category is required"];
                            results.Add(result);
                            continue;
                        }

                        int.TryParse(product.CategoryId, out var categoryId);
                        if (categoryId <= 0)
                        {
                            result.Message = _localizer["Category is invalid"];
                            results.Add(result);
                            continue;
                        }

                        var category = categories.Find(x => x.Id == categoryId);
                        if (category == null)
                        {
                            result.Message = _localizer["Category is invalid"];
                            results.Add(result);
                            continue;
                        }

                        results.Add(result);
                    }

                    var databaseProducts = await _context.Products.Where(x => x.ShopId == request.ShopId).ToListAsync(cancellationToken);
                    var existingProducts = results.Where(c => c.Id != null && c.IsSuccess).ToList();
                    foreach (var product in existingProducts)
                    {
                        var existingProduct = databaseProducts.Find(x => x.Id == product.Id);
                        if (existingProduct == null)
                        {
                            product.Message = _localizer["Product not found"];
                            continue;
                        }

                        existingProduct.Name = product.Name!;
                        existingProduct.Price = decimal.Parse(product.Price!);
                        existingProduct.Stock = int.Parse(product.Stock!);
                        existingProduct.Description = product.Description;
                        existingProduct.CategoryId = int.Parse(product.CategoryId!);
                        _context.Products.Update(existingProduct);
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    var newProducts = results.Where(c => c.Id == null && c.IsSuccess).ToList();
                    foreach (var product in newProducts)
                    {
                        var newProduct = new Product
                        {
                            Name = product.Name!,
                            Price = product.Price,
                            Stock = product.Stock,
                            Description = product.Description,
                            CategoryId = product.CategoryId,
                        };
                        _context.Products.Add(new Domain.Entities.Product
                        {
                            Name = newProduct.Name,
                            Price = decimal.Parse(newProduct.Price!),
                            Stock = int.Parse(newProduct.Stock!),
                            Description = newProduct.Description,
                            CategoryId = int.Parse(newProduct.CategoryId!),
                            ShopId = request.ShopId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsActive = false,
                        });
                        await _context.SaveChangesAsync(cancellationToken);
                    }

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