using SmartCurrencyAssistantWeb.Models;

namespace SmartCurrencyAssistantWeb.Services.Interfaces
{
    public interface ICurrencyConversionService
    {
        Task<CurrencyConversion> ConvertAsync(
            CurrencyConversionRequest request,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CurrencyConversion>> GetHistoryAsync(
            CancellationToken cancellationToken = default);
    }
}