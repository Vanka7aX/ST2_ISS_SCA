using SmartCurrencyAssistantWeb.DTOs;
using SmartCurrencyAssistantWeb.Services.Interfaces;

namespace SmartCurrencyAssistantWeb.Services
{
    public class CurrencyTrendService : ICurrencyTrendService
    {
        private readonly IExchangeRateService _exchangeRateService;

        public CurrencyTrendService(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        public async Task<CurrencyTrendDto> GetTrendAsync(
            string fromCurrency,
            string toCurrency,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken = default)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException(
                    "Start date cannot be after end date.");
            }

            if (startDate > endDate)
            {
                throw new ArgumentException(
                    "Start date cannot be after end date.");
            }

            if (endDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new ArgumentException(
                    "End date cannot be in the future.");
            }


            if (endDate.DayNumber - startDate.DayNumber > 365)
            {
                throw new ArgumentException(
                    "Date range cannot exceed 365 days.");
            }


            if (string.IsNullOrWhiteSpace(fromCurrency) ||
                string.IsNullOrWhiteSpace(toCurrency))
            {
                throw new ArgumentException(
                    "Currency codes are required.");
            }

            fromCurrency = fromCurrency.Trim().ToUpperInvariant();
            toCurrency = toCurrency.Trim().ToUpperInvariant();

            var availableCurrencies = await _exchangeRateService
                .GetAvailableCurrenciesAsync(cancellationToken);

            bool fromSupported = availableCurrencies.Any(currency =>
                string.Equals(
                    currency.Code,
                    fromCurrency,
                    StringComparison.OrdinalIgnoreCase));

            bool toSupported = availableCurrencies.Any(currency =>
                string.Equals(
                    currency.Code,
                    toCurrency,
                    StringComparison.OrdinalIgnoreCase));

            if (!fromSupported)
            {
                throw new ArgumentException(
                    $"Unsupported currency code: {fromCurrency}");
            }

            if (!toSupported)
            {
                throw new ArgumentException(
                    $"Unsupported currency code: {toCurrency}");
            }

            if (fromCurrency == toCurrency)
            {
                return new CurrencyTrendDto
                {
                    FromCurrency = fromCurrency,
                    ToCurrency = toCurrency,
                    StartDate = startDate,
                    EndDate = endDate,
                    StartRate = 1m,
                    EndRate = 1m,
                    AverageRate = 1m,
                    MinRate = 1m,
                    MaxRate = 1m,
                    PercentageChange = 0m,
                    Trend = "Stable"
                };
            }

            var rates = await _exchangeRateService
                .GetHistoricalRatesAsync(fromCurrency, toCurrency, startDate,
                    endDate, cancellationToken);

            var orderedRates = rates.OrderBy(r => r.Date).ToList();

            if (orderedRates.Count == 0)
            {
                throw new InvalidOperationException("No exchange rates found for the selected period.");
            }

            if (orderedRates.Any(r => r.Rate <= 0))
            {
                throw new InvalidOperationException("Invalid exchange rate received.");
            }

            decimal startRate = orderedRates.First().Rate;
            decimal endRate = orderedRates.Last().Rate;

            decimal averageRate = orderedRates.Average(r => r.Rate);
            decimal minRate = orderedRates.Min(r => r.Rate);
            decimal maxRate = orderedRates.Max(r => r.Rate);

            decimal percentageChange = ((endRate - startRate) / startRate) * 100m;

            string trend = endRate > startRate ? "Up" :
                           endRate < startRate ? "Down" :
                           "Stable";

            return new CurrencyTrendDto
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                StartDate = startDate,
                EndDate = endDate,
                StartRate = startRate,
                EndRate = endRate,
                AverageRate = Math.Round(averageRate, 8),
                MinRate = minRate,
                MaxRate = maxRate,
                PercentageChange = Math.Round(percentageChange, 2),
                Trend = trend
            };
        }
    }
}