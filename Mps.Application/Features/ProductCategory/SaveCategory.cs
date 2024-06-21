using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.ProductCategory
{
    public class SaveCategory
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public int? Id { get; set; }
            public required string Name { get; set; }
            public int? ParentId { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<SaveCategory> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<SaveCategory> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.Id.HasValue)
                    {
                        var category = await _context.ProductCategories.FindAsync(request.Id.Value);
                        if (category == null)
                        {
                            return CommandResult<Result>.Fail(_localizer["Category not found"]);
                        }
                        category.Name = request.Name;
                        category.ParentId = request.ParentId;
                        _context.ProductCategories.Update(category);
                    }
                    else
                    {
                        var category = new Domain.Entities.ProductCategory
                        {
                            Name = request.Name,
                            ParentId = request.ParentId
                        };
                        _context.ProductCategories.Add(category);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Category saved successfully"] });
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
