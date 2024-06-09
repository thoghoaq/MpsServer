using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Excel;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace Mps.Application.Features.ProductCategory
{
    public class ImportCategories
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

        public record Category
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
        }

        public class Handler(IExcelService excelService, MpsDbContext context, ILogger<ImportCategories> logger, IAppLocalizer localizer) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly IExcelService _excelService = excelService;
            private readonly MpsDbContext _context = context;
            private readonly ILogger<ImportCategories> _logger = logger;
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
                    var importedCategories = _excelService.ReadExcelFile<Category>(stream);
                    var executeResults = importedCategories.Select((c, index) => ValidateCategory(index, c));

                    var databaseCategories = await _context.ProductCategories.ToListAsync(cancellationToken);
                    var deletedCategories = databaseCategories.Where(c => !executeResults.Any(e => e.Id == c.Id && e.Id != null));
                    if (deletedCategories.Any())
                    {
                        await _context.BulkDeleteAsync(deletedCategories);
                    }

                    var existingCategories = executeResults.Where(c => c.Id != null && c.IsSuccess);
                    if (existingCategories.Any())
                    {
                        await _context.BulkUpdateAsync(existingCategories.Select(c => new Domain.Entities.ProductCategory { Id = c.Id!.Value, Name = c.Name! }));
                    }

                    var newCategories = executeResults.Where(c => c.Id == null && c.IsSuccess);
                    if (newCategories.Any())
                    {
                        await _context.BulkInsertAsync(newCategories.Select(c => new Domain.Entities.ProductCategory { Name = c.Name! }));
                    }

                    return CommandResult<Result>.Success(new Result { Message = _localizer["Imported successfully"], Results = executeResults.ToList() });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }

            private ExecuteResult ValidateCategory(int index, Category category)
            {
                var result = new ExecuteResult { Name = category.Name, Row = index + 2 };
                if (string.IsNullOrEmpty(category.Id))
                {
                    result.Id = null;
                } else if (!int.TryParse(category.Id, out int id) || id <= 0)
                {
                    result.Message = _localizer["Id is not valid"];
                    result.Id = null;
                } else
                {
                    result.Id = id;
                }

                if (category.Name == null)
                {
                    result.Message = _localizer["Name is required"];
                }
                return result;
            }
        }
    }
}
