using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Ecommerce
{
    public class GetProductDetails
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public required int Id { get; set; }
        }

        public class Result
        {
            public required Product Product { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<GetProductDetails> logger) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<GetProductDetails> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var query = await _context.Products
                        .Include(p => p.Images)
                        .Include(p => p.Category)
                        .Include(p => p.Model)
                        .Include(p => p.Shop)
                        .Where(p => p.IsActive)
                        .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                    if (query == null)
                    {
                        return CommandResult<Result>.Fail(_localizer["Not Found"]);
                    }

                    return CommandResult<Result>.Success(new Result { Product = query });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetProductDetailsFailure");
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }
        }
    }
}
