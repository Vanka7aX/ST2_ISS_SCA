using System.Text.Json.Serialization;

namespace SmartCurrencyAssistantWeb.DTOs
{
    public class CurrencyDto
    {
        [JsonPropertyName("iso_code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }
}