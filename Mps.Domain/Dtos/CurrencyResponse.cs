using System.Text.Json.Serialization;

namespace Mps.Domain.Dtos
{
    public class CurrencyResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        [JsonPropertyName("updated_date")]
        public string? UpdatedDate { get; set; }
        [JsonPropertyName("base_currency_code")]
        public string? BaseCurrencyCode { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("base_currency_name")]
        public string? BaseCurrencyName { get; set; }
        [JsonPropertyName("rates")]
        public Dictionary<string, CurrencyRate> Rates { get; set; } = null!;
    }

    public class CurrencyRate
    {
        [JsonPropertyName("currency_name")]
        public string? CurrencyName { get; set; }
        [JsonPropertyName("rate")]
        public string? Rate { get; set; }
        [JsonPropertyName("rate_for_amount")]
        public string? RateForAmount { get; set; }
    }
}
