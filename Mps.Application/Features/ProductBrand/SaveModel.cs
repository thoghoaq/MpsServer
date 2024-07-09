using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.ProductBrand
{
    public class SaveModel
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public int Id { get; set; }
            public int BrandId { get; set; }
            public required string Name { get; set; }
            public string? Cc { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<SaveModel> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<SaveModel> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.Id > 0)
                    {
                        var model = await _context.ProductModels.FindAsync(request.Id);
                        if (model == null)
                        {
                            return CommandResult<Result>.Fail(_localizer["Model not found"]);
                        }
                        model.BrandId = request.BrandId;
                        model.Name = request.Name;
                        model.Cc = request.Cc;
                        model.UpdatedAt = DateTime.UtcNow;
                        _context.ProductModels.Update(model);
                    }
                    else
                    {
                        var model = new Domain.Entities.ProductModel
                        {
                            BrandId = request.BrandId,
                            Name = request.Name,
                            Cc = request.Cc,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.ProductModels.Add(model);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Model saved successfully"] });
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
