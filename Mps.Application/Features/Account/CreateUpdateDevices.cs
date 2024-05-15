using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Abstractions.Localization;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Account
{
    public class CreateUpdateDevices
    {
        public class Command : IRequest<CommandResult<Result>>
        {
            public int? UserDeviceId { get; set; }
            public string? DeviceToken { get; set; }
            public string? DeviceName { get; set; }
            public float? DeviceLatitude { get; set; }
            public float? DeviceLongitude { get; set; }
            public bool? IsLogged { get; set; }
        }

        public class Result
        {
            public string? Message { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILoggedUser loggedUser, IAppLocalizer localizer, ILogger<CreateUpdateDevices> logger) : IRequestHandler<Command, CommandResult<Result>>
        {
            private readonly MpsDbContext _dbContext = dbContext;
            private readonly ILoggedUser _loggedUser = loggedUser;
            private readonly IAppLocalizer _localizer = localizer;
            private readonly ILogger<CreateUpdateDevices> _logger = logger;

            public async Task<CommandResult<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var userId = _loggedUser.UserId;
                    var device = await _dbContext.UserDevices.FirstOrDefaultAsync(d => (d.UserDeviceId == request.UserDeviceId || d.DeviceToken == request.DeviceToken) && d.UserId == userId, cancellationToken);
                    if (device == null)
                    {
                        device = new UserDevice
                        {
                            UserId = userId,
                            DeviceToken = request.DeviceToken,
                            DeviceName = request.DeviceName,
                            DeviceLatitude = request.DeviceLatitude,
                            DeviceLongitude = request.DeviceLongitude,
                            IsLogged = request.IsLogged ?? true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        await _dbContext.UserDevices.AddAsync(device, cancellationToken);
                    }
                    else
                    {
                        if (request.DeviceToken != null) device.DeviceToken = request.DeviceToken;
                        if (request.DeviceName != null) device.DeviceName = request.DeviceName;
                        if (request.DeviceLatitude != null) device.DeviceLatitude = request.DeviceLatitude;
                        if (request.DeviceLongitude != null) device.DeviceLongitude = request.DeviceLongitude;
                        if (request.IsLogged != null) device.IsLogged = request.IsLogged ?? true;
                        device.UpdatedAt = DateTime.UtcNow;
                    }
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return CommandResult<Result>.Success(new Result { Message = _localizer["Update device successfully"] });
                } catch (Exception ex)
                {
                    _logger.LogError(ex, "CreateUpdateDevicesFailure");
                    return CommandResult<Result>.Fail(ex.Message);
                }
            }
        }
    }
}
