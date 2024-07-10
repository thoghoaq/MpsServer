using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Setting
{
    public class SaveSetting
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public required string Key { get; set; }
            public required string Value { get; set; }
            public string? Description { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext context, IAppLocalizer localizer, ILogger<SaveSetting> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _context = context;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<SaveSetting> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var setting = await _context.Settings.FirstOrDefaultAsync(e => e.Key == request.Key);
                    if (setting == null)
                    {
                        setting = new Domain.Entities.Setting { Key = request.Key, Value = request.Value, Description = request.Description };
                        await _context.Settings.AddAsync(setting, cancellationToken);
                    }

                    setting.Value = request.Value;
                    setting.Description = request.Description;
                    await _context.SaveChangesAsync(cancellationToken);

                    return CommandResult<Result>.Success(new Result { Message = _localizer["Setting saved"] });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when save setting");
                    return CommandResult<Result>.Fail(_localizer["Error when save setting"]);
                }
            }
        }
    }
}
