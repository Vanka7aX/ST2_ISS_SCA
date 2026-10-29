using SmartCurrencyAssistantWeb.DTOs;
using SmartCurrencyAssistantWeb.Services.Interfaces;
//using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;

namespace SmartCurrencyAssistantWeb.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public ExchangeRateService(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        public async Task<ExchangeRateDto> GetLatestRateAsync(string fromCurrency, 
            string toCurrency, CancellationToken cancellationToken = default)
        {
            fromCurrency = NormalizeCurrency(fromCurrency);
            toCurrency = NormalizeCurrency(toCurrency);

            string url = $"https://api.frankfurter.dev/v2/rate/{fromCurrency}/{toCurrency}";

            var result = await _httpClient.GetFromJsonAsync<ExchangeRateDto>(url, 
                cancellationToken);

            return result ?? throw new InvalidOperationException("The API returned an empty response.");
        }

        public async Task<IReadOnlyList<ExchangeRateDto>> GetHistoricalRatesAsync(
            string fromCurrency,
            string toCurrency,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken = default)
        {
            fromCurrency = NormalizeCurrency(fromCurrency);
            toCurrency = NormalizeCurrency(toCurrency);

            if (startDate > endDate)
            {
                throw new ArgumentException(
                    "Start date cannot be after end date.");
            }

            string url =
                $"https://api.frankfurter.dev/v2/rates" +
                $"?from={startDate:yyyy-MM-dd}" +
                $"&to={endDate:yyyy-MM-dd}" +
                $"&base={fromCurrency}" +
                $"&quotes={toCurrency}";

            var result = await _httpClient
                .GetFromJsonAsync<List<ExchangeRateDto>>(
                    url, cancellationToken);

            return result?.OrderBy(r => r.Date).ToList()
                ?? throw new InvalidOperationException(
                    "The API returned an empty response.");
        }

        public async Task<IReadOnlyList<CurrencyDto>> GetAvailableCurrenciesAsync(
            CancellationToken cancellationToken = default)
        {
            const string cacheKey = "FrankfurterAvailableCurrencies";

            if (_memoryCache.TryGetValue(cacheKey, out IReadOnlyList<CurrencyDto>? cachedCurrencies)
                && cachedCurrencies != null)
            {
                return cachedCurrencies;
            }

            const string url = "https://api.frankfurter.dev/v2/currencies";

            var currencies = await _httpClient
                .GetFromJsonAsync<List<CurrencyDto>>(
                    url, cancellationToken);

            if (currencies == null)
            {
                throw new InvalidOperationException("The API returned an empty response.");
            }

            IReadOnlyList<CurrencyDto> sortedCurrencies = currencies.OrderBy(c => c.Name)
                .ToList();

            _memoryCache.Set(cacheKey, sortedCurrencies, TimeSpan.FromHours(24));

            return sortedCurrencies;
        }

        private static string NormalizeCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException(
                    "Currency code is required.");
            }

            string code = currency.Trim().ToUpperInvariant();

            if (code.Length != 3 || code.Any(c => c < 'A' || c > 'Z'))
            {
                throw new ArgumentException(
                    "Currency code must contain three letters.");
            }
            return code;
        }
    }
}