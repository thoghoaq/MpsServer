namespace Mps.Domain.Entities
{
    public class UserDevice
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? DeviceToken { get; set; }
        public string? DeviceName { get; set; }
        public float? DeviceLatitude { get; set; }
        public float? DeviceLongitude { get; set; }

        public bool IsLogged { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
    }
}
