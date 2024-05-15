using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Messaging;
using Mps.Domain.Entities;

namespace Mps.Infrastructure.Dependencies.Firebase.Messaging
{
    public class FirebaseNotificationService(MpsDbContext dbContext, ILogger<FirebaseNotificationService> logger) : INotificationService
    {
        private readonly MpsDbContext _dbContext = dbContext;
        private readonly ILogger<FirebaseNotificationService> _logger = logger;

        public async Task<MessageResponse> SendMessageAsync(MessageRequest request)
        {
            try
            {
                var messaging = FirebaseMessaging.DefaultInstance;
                var result = await messaging.SendAsync(new Message
                {
                    Notification = new Notification
                    {
                        Title = request.Title,
                        Body = request.Body,
                        ImageUrl = request.ImageUrl,
                    },
                    Token = request.DeviceToken,
                    Data = request.Data,
                });
                return new MessageResponse
                {
                    Success = true,
                    Message = result,
                };
            } catch (Exception ex)
            {
                _logger.LogError(ex, "SendMessageFailure");
                return new MessageResponse
                {
                    Success = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<List<MessageResponse>> SendMessageAllDevicesAsync(int userId, MessageRequest request)
        {
            try
            {
                var devices = _dbContext.UserDevices.Where(d => d.UserId == userId && d.IsLogged == true).ToList();
                if (devices == null || devices.Count == 0)
                {
                    return [
                        new MessageResponse
                        {
                            Success = false,
                            Message = "No device found",
                        }
                    ];
                }
                var messaging = FirebaseMessaging.DefaultInstance;
                var result = await messaging.SendEachForMulticastAsync(new MulticastMessage
                {
                    Notification = new Notification
                    {
                        Title = request.Title,
                        Body = request.Body,
                        ImageUrl = request.ImageUrl,
                    },
                    Tokens = devices.Select(d => d.DeviceToken).ToList(),
                    Data = request.Data,
                });
                return result.Responses.Select(r => new MessageResponse
                {
                    Success = r.IsSuccess,
                    Message = r.Exception?.Message,
                }).ToList();
            } catch (Exception ex)
            {
                _logger.LogError(ex, "SendMessageAllDevicesFailure");
                return [
                    new MessageResponse
                    {
                        Success = false,
                        Message = ex.Message,
                    }
                ];
            }
        }
    }
}
