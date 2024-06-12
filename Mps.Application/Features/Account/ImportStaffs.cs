using EFCore.BulkExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Excel;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;
using Mps.Domain.Enums;
using Mps.Domain.Extensions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Mps.Application.Features.Account
{
    public class ImportStaffs
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

        public class ExecuteResult
        {
            public int Row { get; set; }
            public Data? Data { get; set; }
            public bool IsSuccess
            {
                get
                {
                    return string.IsNullOrEmpty(Message);
                }
                set
                {
                    Message = value ? null : Message;
                }
            }

            public string? Message { get; set; }
        }

        public record Data
        {
            public string? Id { get; set; }
            public string? FullName { get; set; }
            public string? Email { get; set; }
            public string? AvatarPath { get; set; }
            public string? PhoneNumber { get; set; }

            public string? IdentityCard { get; set; }
            public string? IdentityCardFrontPath { get; set; }
            public string? IdentityCardBackPath { get; set; }
            public string? Address { get; set; }
            public string? CertificatePath { get; set; }
        }

        public class Handler(IExcelService excelService, MpsDbContext context, ILogger<ImportStaffs> logger, IAppLocalizer localizer, IMediator mediator) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly IExcelService _excelService = excelService;
            private readonly MpsDbContext _context = context;
            private readonly ILogger<ImportStaffs> _logger = logger;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly IMediator _mediator = mediator;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if (request.File == null || request.File.Length == 0)
                    {
                        return CommandResult<Result>.Fail(_localizer["File is empty"]);
                    }

                    using var stream = new MemoryStream();
                    await request.File.CopyToAsync(stream, cancellationToken);
                    stream.Position = 0;
                    var importedStaffs = _excelService.ReadExcelFile<Data>(stream);
                    var executeResults = new List<ExecuteResult>();
                    foreach (var (staff, index) in importedStaffs.Select((c, index) => (c, index)))
                    {
                        var result = Validate(index, staff);
                        executeResults.Add(result);
                    }

                    var existedStaffs = executeResults.Where(c => c.IsSuccess && c.Data?.Id != null);
                    if (existedStaffs.Any())
                    {
                        var ids = existedStaffs.Select(c => int.Parse(c.Data?.Id ?? "0")).ToList();
                        var dbUser = await _context.Users.Include(u => u.Staff).Where(u => ids.Contains(u.Id)).ToListAsync(cancellationToken);
                        dbUser.ForEach(u =>
                        {
                            var staff = existedStaffs.FirstOrDefault(c => c.Data?.Id == u.Id.ToString());
                            if (staff != null)
                            {
                                u.FullName = staff.Data?.FullName!;
                                u.Email = staff.Data?.Email!;
                                u.AvatarPath = staff.Data?.AvatarPath;
                                u.PhoneNumber = staff.Data?.PhoneNumber;
                                u.Staff!.IdentityCardFrontPath = staff.Data?.IdentityCardFrontPath;
                                u.Staff!.IdentityCardBackPath = staff.Data?.IdentityCardBackPath;
                                u.Staff!.IdentityCard = staff.Data?.IdentityCard;
                                u.Staff!.Address = staff.Data?.Address;
                                u.Staff!.CertificatePath = staff.Data?.CertificatePath;
                                
                                u.UpdatedAt = DateTime.UtcNow;
                                u.Staff.UpdatedAt = DateTime.UtcNow;
                            }
                        });
                        await _context.SaveChangesAsync(cancellationToken);
                    }

                    foreach (var c in executeResults.Where(c => c.IsSuccess && c.Data?.Id == null))
                    {
                        var command = new CreateUser.Command
                        {
                            Email = c.Data?.Email!,
                            FullName = c.Data?.FullName!,
                            PhoneNumber = c.Data?.PhoneNumber,
                            AvatarPath = c.Data?.AvatarPath,
                            Role = Role.Staff.GetDescription(),
                            StaffData = new CreateUser.StaffData
                            {
                                IdentityCard = c.Data?.IdentityCard,
                                IdentityCardFrontPath = c.Data?.IdentityCardFrontPath,
                                IdentityCardBackPath = c.Data?.IdentityCardBackPath,
                                Address = c.Data?.Address,
                                CertificatePath = c.Data?.CertificatePath
                            }
                        };
                        var result = await _mediator.Send(command, cancellationToken);
                        c.Message = result.FailureReason;
                        c.IsSuccess = result.IsSuccess;
                    }

                    if (executeResults.Any(c => !c.IsSuccess))
                    {
                        return CommandResult<Result>.Success(new Result { Message = _localizer["Import partial successfully"], Results = executeResults.ToList() });
                    }
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Imported successfully"], Results = executeResults.ToList() });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return CommandResult<Result>.Fail(_localizer[ex.Message]);
                }
            }

            private ExecuteResult Validate(int index, Data data)
            {
                var result = new ExecuteResult { 
                    Data = data,
                    Row = index + 2
                };
                bool idIsInt = int.TryParse(data.Id, out int id);
                if (string.IsNullOrEmpty(data.Id))
                {
                    result.Data.Id = null;
                }
                else if (!idIsInt || id <= 0)
                {
                    result.Message = _localizer["Id is not valid"];
                    result.Data.Id = null;
                }

                if (data.Email == null)
                {
                    result.Message = _localizer["Email is required"];
                }
                else if (!Regex.IsMatch(data.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                {
                    result.Message = _localizer["Email is not valid"];
                }
                else
                {
                    var existed = _context.Users.Any(x => x.Email == data.Email && x.Id != id);
                    if (existed && result.Data.Id != null)
                    {
                        result.Message = _localizer["Email is existed"];
                    }
                }

                if (data.FullName == null)
                {
                    result.Message = _localizer["Name is required"];
                } else if (data.FullName.Length < 3)
                {
                    result.Message = _localizer["Name must be 3 charactor above"];
                } else if (!Regex.IsMatch(data.FullName, @"^[\p{L}\p{M}]+([\p{L}\p{M} '-]*[\p{L}\p{M}]+)*$"))
                {
                    result.Message = _localizer["Name is not valid"];
                }

                if (!string.IsNullOrEmpty(data.PhoneNumber) && !Regex.IsMatch(data.PhoneNumber, @"\+?[1-9]\d{1,14}|\(?\d{1,4}\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}[-.\s]?\d{1,9}"))
                {
                    result.Message = _localizer["Phone number is not valid"];
                }

                return result;
            }
        }
    }
}
