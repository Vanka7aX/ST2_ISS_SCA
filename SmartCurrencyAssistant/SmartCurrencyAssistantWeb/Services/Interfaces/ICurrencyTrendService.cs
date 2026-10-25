using SmartCurrencyAssistantWeb.DTOs;

namespace SmartCurrencyAssistantWeb.Services.Interfaces
{
    public interface ICurrencyTrendService
    {
        Task<CurrencyTrendDto> GetTrendAsync(
            string fromCurrency,
            string toCurrency,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken = default);
    }
}