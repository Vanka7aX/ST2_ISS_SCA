using System.ComponentModel.DataAnnotations;

namespace SmartCurrencyAssistantWeb.Models
{
    public class CurrencyConversionRequest
    {
        [Required]
        [Range(typeof(decimal), "0.01", "1000000000")]
        public decimal Amount { get; set; }

        [Required]
        public string FromCurrency { get; set; } = "EUR";

        [Required]
        public string ToCurrency { get; set; } = "USD";
    }
}