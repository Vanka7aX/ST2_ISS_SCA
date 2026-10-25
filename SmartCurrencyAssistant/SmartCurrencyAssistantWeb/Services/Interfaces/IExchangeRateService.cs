using SmartCurrencyAssistantWeb.DTOs;

namespace SmartCurrencyAssistantWeb.Services.Interfaces
{
    public interface IExchangeRateService
    {
        Task<ExchangeRateDto> GetLatestRateAsync(
            string fromCurrency,
            string toCurrency,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ExchangeRateDto>> GetHistoricalRatesAsync(
            string fromCurrency,
            string toCurrency,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CurrencyDto>> GetAvailableCurrenciesAsync(
            CancellationToken cancellationToken = default);
    }
}