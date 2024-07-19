namespace Mps.Application.Abstractions.Messaging
{
    public interface INotificationService
    {
        public Task<MessageResponse> SendMessageAsync(MessageRequest request);
        public Task<List<MessageResponse>> SendMessageAllDevicesAsync(int userId, MessageRequest request);
        public Task<List<MessageResponse>> SendMessageAllDevicesAsync(List<int> userIds, MessageRequest request);
    }
}
