using System.Text.Json.Serialization;

namespace SmartCurrencyAssistantWeb.DTOs
{
    public class ExchangeRateDto
    {
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; } = string.Empty;

        [JsonPropertyName("quote")]
        public string Quote { get; set; } = string.Empty;

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
    }
}