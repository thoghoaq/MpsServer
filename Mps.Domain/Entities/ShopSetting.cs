namespace Mps.Domain.Entities
{
    public class ShopSetting
    {
        public int Id { get; set; }
        public required string Key { get; set; }
        public required string Value { get; set; }
        public string? Description { get; set; }
        public required int ShopId { get; set; }

        public Shop? Shop { get; set; }
    }
}
