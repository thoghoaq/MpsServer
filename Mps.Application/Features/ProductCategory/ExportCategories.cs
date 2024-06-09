using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Excel;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;

namespace Mps.Application.Features.ProductCategory
{
    public class ExportCategories
    {
        public class Query : IRequest<CommandResult<Result>>
        {
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public string? Filter { get; set; }
        }

        public class Result
        {
            public required MemoryStream FileStream { get; set; }
        }

        public class Handler(IExcelService excelService, IMediator mediator, IAppLocalizer localizer, ILogger<ExportCategories> logger) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly IExcelService _excelService = excelService;
            private readonly IMediator _mediator = mediator;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<ExportCategories> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                { 
                    var result = await _mediator.Send(new GetCategories.Query
                    {
                        Filter = request.Filter,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize
                    }, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        return CommandResult<Result>.Fail(_localizer[result.FailureReason ?? "Error when get categories"]);
                    }

                    var fileStream = _excelService.ExportToExcel(result.Payload?.Categories ?? []);
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
