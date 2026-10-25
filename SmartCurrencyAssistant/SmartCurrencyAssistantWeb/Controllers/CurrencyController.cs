using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCurrencyAssistantWeb.Models;
using SmartCurrencyAssistantWeb.Services.Interfaces;
using SmartCurrencyAssistantWeb.DTOs;

namespace SmartCurrencyAssistantWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyConversionService _conversionService;
        private readonly IExchangeRateService _exchangeRateService;

        public CurrencyController(ICurrencyConversionService conversionService, 
            IExchangeRateService exchangeRateService)
        {
            _conversionService = conversionService;
            _exchangeRateService = exchangeRateService;
        }

        [HttpPost("convert")]
        public async Task<ActionResult<CurrencyConversion>> Convert(
            [FromBody] CurrencyConversionRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _conversionService.ConvertAsync(
                request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IReadOnlyList<CurrencyConversion>>> GetHistory(
            CancellationToken cancellationToken)
        {
            var history = await _conversionService.GetHistoryAsync(
                cancellationToken);

            return Ok(history);
        }

        [HttpGet("rates/history")]
        public async Task<ActionResult<IReadOnlyList<ExchangeRateDto>>> GetHistoricalRates(
        [FromQuery] string fromCurrency,
        [FromQuery] string toCurrency,
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
        {
            if (startDate > endDate)
            {
                return BadRequest(
                    "Start date cannot be after end date.");
            }

            if (endDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                return BadRequest(
                    "End date cannot be in the future.");
            }

            var rates = await _exchangeRateService
                .GetHistoricalRatesAsync(
                    fromCurrency,
                    toCurrency,
                    startDate,
                    endDate,
                    cancellationToken);

            return Ok(rates);
        }

        [HttpGet("currencies")]
        public async Task<ActionResult<IReadOnlyList<CurrencyDto>>> GetAvailableCurrencies(
            CancellationToken cancellationToken)
        {
            var currencies = await _exchangeRateService
                .GetAvailableCurrenciesAsync(cancellationToken);

            return Ok(currencies);
        }
    }
}