using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SmartCurrencyAssistantWeb.Models
{
    public class CurrencyConversion
    {
        [Key]
        public int Id { get; set; }

        [Precision(18, 4)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3)]
        public string FromCurrency { get; set; } = string.Empty;

        [Required]
        [StringLength(3)]
        public string ToCurrency { get; set; } = string.Empty;

        [Precision(18, 8)]
        public decimal ExchangeRate { get; set; }

        [Precision(18, 4)]
        public decimal ConvertedAmount { get; set; }

        public DateTime ConvertedAtUtc { get; set; } = DateTime.UtcNow;
    }
}