using MediatR;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Excel;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Enums;
using Mps.Domain.Extensions;

namespace Mps.Application.Features.Account
{
    public class ExportStaffs
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

        public record StaffData
        {
            public int Id { get; set; }
            public string? StaffCode { get; set; }
            public required string FullName { get; set; }
            public required string Email { get; set; }
            public string? AvatarPath { get; set; }
            public string? PhoneNumber { get; set; }

            public string? IdentityCard { get; set; }
            public string? IdentityCardFrontPath { get; set; }
            public string? IdentityCardBackPath { get; set; }
            public string? Address { get; set; }
            public string? CertificatePath { get; set; }

            public required string IdentityId { get; set; }
            public string? CreatedAt { get; set; }
            public string? UpdatedAt { get; set; }
        }

        public class Handler(IExcelService excelService, IMediator mediator, IAppLocalizer localizer, ILogger<ExportStaffs> logger) : IRequestHandler<Query, CommandResult<Result>>
        {
            private readonly IExcelService _excelService = excelService;
            private readonly IMediator _mediator = mediator;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<ExportStaffs> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = await _mediator.Send(new GetAllUsers.Query
                    {
                        Filter = request.Filter,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        Role = Role.Staff.GetDescription()
                    }, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        return CommandResult<Result>.Fail(_localizer[result.FailureReason!]);
                    }

                    var staffs = result.Payload?.Users.Select(x => new StaffData
                    {
                        Id = x.Id,
                        FullName = x.FullName,
                        StaffCode = x.Staff?.StaffCode,
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        IdentityId = x.IdentityId,
                        Address = x.Staff?.Address,
                        CertificatePath = x.Staff?.CertificatePath,
                        IdentityCard = x.Staff?.IdentityCard,
                        IdentityCardFrontPath = x.Staff?.IdentityCardFrontPath,
                        IdentityCardBackPath = x.Staff?.IdentityCardBackPath,
                        AvatarPath = x.AvatarPath,
                        CreatedAt = x.Staff?.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                        UpdatedAt = x.Staff?.UpdatedAt?.ToString("dd/MM/yyyy HH:mm")
                    }).ToList();

                    var fileStream = _excelService.ExportToExcel(staffs ?? []);
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
