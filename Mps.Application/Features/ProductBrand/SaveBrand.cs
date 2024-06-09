using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.ProductBrand
{
    public class SaveBrand
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public int? Id { get; set; }
            public required string Name { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<SaveBrand> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<SaveBrand> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.Id.HasValue)
                    {
                        var brand = await _context.ProductBrands.FindAsync(request.Id.Value);
                        if (brand == null)
                        {
                            return CommandResult<Result>.Fail(_localizer["Brand not found"]);
                        }
                        brand.Name = request.Name;
                        _context.ProductBrands.Update(brand);
                    }
                    else
                    {
                        var brand = new Domain.Entities.ProductBrand { Name = request.Name };
                        _context.ProductBrands.Add(brand);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Brand saved successfully"] });
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
