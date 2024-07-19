using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Mps.Application.Abstractions.Messaging;
using Mps.Domain.Entities;
using Newtonsoft.Json;

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
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = request.Title,
                        Body = request.Body,
                        ImageUrl = request.ImageUrl,
                    },
                    Token = request.DeviceToken,
                    Data = request.Data,
                });

                var userId = _dbContext.UserDevices.FirstOrDefault(d => d.DeviceToken == request.DeviceToken)?.UserId;
                if (userId != null)
                {
                    _dbContext.Notifications.Add(new Domain.Entities.UserNotification
                    {
                        Title = request.Title,
                        Body = request.Body,
                        ImageUrl = request.ImageUrl,
                        UserId = (int)userId,
                        Data = JsonConvert.SerializeObject(request.Data),
                        CreatedAt = DateTime.UtcNow,
                    });
                    await _dbContext.SaveChangesAsync();
                }
                return new MessageResponse
                {
                    Success = true,
                    Message = result,
                };
            }
            catch (Exception ex)
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
                var devices = _dbContext.UserDevices.Where(d => d.UserId == userId && d.IsLogged).ToList();
                if (devices.Count == 0)
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
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = request.Title,
                        Body = request.Body,
                        ImageUrl = request.ImageUrl,
                    },
                    Tokens = devices.Select(d => d.DeviceToken).ToList(),
                    Data = request.Data,
                });

                _dbContext.Notifications.Add(new Domain.Entities.UserNotification
                {
                    Title = request.Title,
                    Body = request.Body,
                    ImageUrl = request.ImageUrl,
                    UserId = userId,
                    Data = JsonConvert.SerializeObject(request.Data),
                    CreatedAt = DateTime.UtcNow,
                });
                await _dbContext.SaveChangesAsync();

                return result.Responses.Select(r => new MessageResponse
                {
                    Success = r.IsSuccess,
                    Message = r.Exception?.Message,
                }).ToList();
            }
            catch (Exception ex)
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

        public async Task<List<MessageResponse>> SendMessageAllDevicesAsync(List<int> userIds, MessageRequest request)
        {
            try
            {
                var devices = _dbContext.UserDevices.Where(d => userIds.Contains(d.UserId) && d.IsLogged).ToList();
                if (devices.Count == 0)
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
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = request.Title,
                        Body = request.Body,
                        ImageUrl = request.ImageUrl,
                    },
                    Tokens = devices.Select(d => d.DeviceToken).ToList(),
                    Data = request.Data,
                });

                _dbContext.Notifications.AddRange(userIds.Select(u => new Domain.Entities.UserNotification
                {
                    Title = request.Title,
                    Body = request.Body,
                    ImageUrl = request.ImageUrl,
                    UserId = u,
                    Data = JsonConvert.SerializeObject(request.Data),
                    CreatedAt = DateTime.UtcNow,
                }).ToList());
                await _dbContext.SaveChangesAsync();

                return result.Responses.Select(r => new MessageResponse
                {
                    Success = r.IsSuccess,
                    Message = r.Exception?.Message,
                }).ToList();
            }
            catch (Exception ex)
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
