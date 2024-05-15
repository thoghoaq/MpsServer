using MediatR;
using Microsoft.EntityFrameworkCore;
using Mps.Application.Abstractions.Authentication;
using Mps.Application.Commons;
using Mps.Domain.Entities;

namespace Mps.Application.Features.Account
{
    public class GetDevices
    {
        public class Query : IRequest<CommandResult<List<Result>>>
        {
        }

        public class Result
        {
            public int UserDeviceId { get; set; }
            public int UserId { get; set; }
            public string? DeviceToken { get; set; }
            public string? DeviceName { get; set; }
            public float? DeviceLatitude { get; set; }
            public float? DeviceLongitude { get; set; }

            public bool IsLogged { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class Handler(MpsDbContext dbContext, ILoggedUser loggedUser) : IRequestHandler<Query, CommandResult<List<Result>>>
        {
            private readonly MpsDbContext _dbContext = dbContext;
            private readonly ILoggedUser _loggedUser = loggedUser;

            public async Task<CommandResult<List<Result>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _loggedUser.UserId;
                var device = await _dbContext.UserDevices.Where(d => d.UserId == userId).ToListAsync(cancellationToken);
                var result = device.Select(d => new Result
                {
                    UserDeviceId = d.UserDeviceId,
                    UserId = d.UserId,
                    DeviceToken = d.DeviceToken,
                    DeviceName = d.DeviceName,
                    DeviceLatitude = d.DeviceLatitude,
                    DeviceLongitude = d.DeviceLongitude,
                    IsLogged = d.IsLogged,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                }).ToList();
                return CommandResult<List<Result>>.Success(result);
            }
        }
    }
}
