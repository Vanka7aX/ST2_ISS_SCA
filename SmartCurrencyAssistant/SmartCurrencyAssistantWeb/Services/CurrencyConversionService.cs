
using Microsoft.EntityFrameworkCore;
using SmartCurrencyAssistantWeb.Data;
using SmartCurrencyAssistantWeb.Models;
using SmartCurrencyAssistantWeb.Services.Interfaces;

namespace SmartCurrencyAssistantWeb.Services
{
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExchangeRateService _exchangeRateService;

        public CurrencyConversionService(
            ApplicationDbContext context,
            IExchangeRateService exchangeRateService)
        {
            _context = context;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<CurrencyConversion> ConvertAsync(
            CurrencyConversionRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.Amount < 0.01m ||
                request.Amount > 1_000_000_000m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(request.Amount),
                    "Amount must be between 0.01 and 1,000,000,000.");
            }

            string fromCurrency =
                NormalizeCurrency(request.FromCurrency);

            string toCurrency =
                NormalizeCurrency(request.ToCurrency);

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

            decimal amount = decimal.Round(
                request.Amount, 4, MidpointRounding.AwayFromZero);

            decimal exchangeRate = 1m;
            DateOnly rateDate =
                DateOnly.FromDateTime(DateTime.UtcNow);

            if (fromCurrency != toCurrency)
            {
                var rate = await _exchangeRateService
                    .GetLatestRateAsync(
                        fromCurrency,
                        toCurrency,
                        cancellationToken);

                if (rate.Rate <= 0)
                {
                    throw new InvalidOperationException(
                        "Invalid exchange rate received.");
                }

                exchangeRate = rate.Rate;
                rateDate = rate.Date;
            }

            decimal convertedAmount = decimal.Round(
                amount * exchangeRate,
                4,
                MidpointRounding.AwayFromZero);

            var conversion = new CurrencyConversion
            {
                Amount = amount,
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                ExchangeRate = exchangeRate,
                ConvertedAmount = convertedAmount,
                RateDate = rateDate,
                ConvertedAtUtc = DateTime.UtcNow
            };

            _context.CurrencyConversions.Add(conversion);

            await _context.SaveChangesAsync(cancellationToken);

            return conversion;
        }

        public async Task<IReadOnlyList<CurrencyConversion>>
            GetHistoryAsync(
                CancellationToken cancellationToken = default)
        {
            return await _context.CurrencyConversions
                .AsNoTracking()
                .OrderByDescending(c => c.ConvertedAtUtc)
                .ThenByDescending(c => c.Id)
                .Take(100)
                .ToListAsync(cancellationToken);
        }

        private static string NormalizeCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException(
                    "Currency code is required.");
            }

            string code = currency.Trim().ToUpperInvariant();

            if (code.Length != 3 ||
                code.Any(c => c < 'A' || c > 'Z'))
            {
                throw new ArgumentException(
                    "Currency code must contain three letters.");
            }

            return code;
        }
    }
}