namespace SmartCurrencyAssistantWeb.DTOs
    {
    public class CurrencyTrendDto
    {
        public string FromCurrency { get; set; } = string.Empty;

        public string ToCurrency { get; set; } = string.Empty;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public decimal StartRate { get; set; }

        public decimal EndRate { get; set; }

        public decimal AverageRate { get; set; }

        public decimal MinRate { get; set; }

        public decimal MaxRate { get; set; }

        public decimal PercentageChange { get; set; }

        public string Trend { get; set; } = string.Empty;
    }
}