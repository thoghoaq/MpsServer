namespace Mps.Application.Abstractions.Messaging
{
    public class MessageRequest
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
        public string? ImageUrl { get; set; }
        public string? DeviceToken { get; set; }
        public Dictionary<string, string>? Data { get; set; }
    }
}
