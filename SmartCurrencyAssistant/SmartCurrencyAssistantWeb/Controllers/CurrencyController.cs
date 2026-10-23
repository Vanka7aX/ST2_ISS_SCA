using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCurrencyAssistantWeb.Models;
using SmartCurrencyAssistantWeb.Services.Interfaces;

namespace SmartCurrencyAssistantWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyConversionService _conversionService;

        public CurrencyController(
            ICurrencyConversionService conversionService)
        {
            _conversionService = conversionService;
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
    }
}