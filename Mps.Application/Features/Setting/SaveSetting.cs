﻿using MediatR;
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
            public List<Request> Settings { get; set; } = [];
        }

        public record Request
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
                    // check any change
                    var settings = await _context.Settings.ToListAsync(cancellationToken);
                    var changedSettings = request.Settings.Where(s => settings.Any(x => x.Key == s.Key && (x.Value != s.Value || x.Description != s.Description))).ToList();
                    if (changedSettings.Count == 0)
                    {
                        return CommandResult<Result>.Success(new Result { Message = _localizer["No setting changed"] });
                    }
                    // save new setting base on key
                    foreach (var setting in changedSettings)
                    {
                        var existSetting = settings.Find(s => s.Key == setting.Key);
                        if (existSetting != null)
                        {
                            existSetting.Value = setting.Value;
                            existSetting.Description = setting.Description;
                        }
                        else
                        {
                            _context.Settings.Add(new Domain.Entities.Setting
                            {
                                Key = setting.Key,
                                Value = setting.Value,
                                Description = setting.Description
                            });
                        }
                    }
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
