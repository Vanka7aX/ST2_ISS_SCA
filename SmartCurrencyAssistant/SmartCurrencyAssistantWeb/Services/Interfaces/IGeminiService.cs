using SmartCurrencyAssistantWeb.DTOs;

namespace SmartCurrencyAssistantWeb.Services.Interfaces
{
    public interface IGeminiService
    {
        Task<string> AnalyzeTrendAsync(
            CurrencyTrendDto trend,
            CancellationToken cancellationToken = default);
    }
}