namespace Mps.Domain.Entities
{
    public class Setting
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
        public string? Description { get; set; }
    }
}
