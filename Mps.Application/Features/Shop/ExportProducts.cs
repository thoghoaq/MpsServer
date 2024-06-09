using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Excel;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;

namespace Mps.Application.Features.Shop
{
    public class ExportProducts
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public required int ShopId { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
        }

        public class Result
        {
            public required MemoryStream FileStream { get; set; }
        }

        public class Handler(IExcelService excelService, IMediator mediator, IAppLocalizer localizer, ILogger<ExportProducts> logger) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly IExcelService _excelService = excelService;
            private readonly IMediator _mediator = mediator;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<ExportProducts> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = await _mediator.Send(new GetProducts.Query
                    {
                        ShopId = request.ShopId,
                        Filter = request.Filter,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize
                    }, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        return CommandResult<Result>.Fail(_localizer[result.FailureReason ?? "Error when get products"]);
                    }

                    var fileStream = _excelService.ExportToExcel(result.Payload?.Products ?? []);
                    return CommandResult<Result>.Success(new Result
                    {
                        FileStream = fileStream
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
