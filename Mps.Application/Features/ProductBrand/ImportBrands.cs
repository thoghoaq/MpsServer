using EFCore.BulkExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Excel;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.ProductBrand
{
    public class ImportBrands
    {
        public class Command : IRequest<CommandResult<Result>>
        {
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
            public bool IsSuccess => string.IsNullOrEmpty(Message);
            public string? Message { get; set; }
        }

        public record Brand
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
        }

        public class Handler(IExcelService excelService, MpsDbContext context, ILogger<ImportBrands> logger, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly IExcelService _excelService = excelService;
            private readonly MpsDbContext _context = context;
            private readonly ILogger<ImportBrands> _logger = logger;
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
                    var importedBrands = _excelService.ReadExcelFile<Brand>(stream);
                    var executeResults = importedBrands.Select((c, index) => ValidateBrand(index, c));

                    var databaseBrands = await _context.ProductBrands.ToListAsync(cancellationToken);
                    var deletedBrands = databaseBrands.Where(c => !executeResults.Any(e => e.Id == c.Id && e.Id != null));
                    if (deletedBrands.Any())
                    {
                        await _context.BulkDeleteAsync(deletedBrands.ToList());
                    }

                    var existingBrands = executeResults.Where(c => c.Id != null && c.IsSuccess);
                    if (existingBrands.Any())
                    {
                        await _context.BulkUpdateAsync(existingBrands.Select(c => new Domain.Entities.ProductBrand { Id = c.Id!.Value, Name = c.Name! }).ToList());
                    }

                    var newBrands = executeResults.Where(c => c.Id == null && c.IsSuccess);
                    if (newBrands.Any())
                    {
                        await _context.BulkInsertAsync(newBrands.Select(c => new Domain.Entities.ProductBrand { Name = c.Name! }).ToList());
                    }

                    return CommandResult<Result>.Success(new Result { Message = _localizer["Imported successfully"], Results = executeResults.ToList() });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(_localizer["An error occurred while importing brands"]);
                }
            }

            private ExecuteResult ValidateBrand(int index, Brand brand)
            {
                var result = new ExecuteResult { Name = brand.Name, Row = index + 2 };
                if (string.IsNullOrEmpty(brand.Id))
                {
                    result.Id = null;
                }
                else if (!int.TryParse(brand.Id, out int id) || id <= 0)
                {
                    result.Message = _localizer["Id is not valid"];
                    result.Id = null;
                }
                else
                {
                    result.Id = id;
                }

                if (brand.Name == null)
                {
                    result.Message = _localizer["Name is required"];
                }
                return result;
            }
        }
    }
}
