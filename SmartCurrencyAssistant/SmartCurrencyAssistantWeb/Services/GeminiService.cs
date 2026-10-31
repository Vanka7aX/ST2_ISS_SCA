
using System.Net.Http.Json;
using System.Text.Json;
using SmartCurrencyAssistantWeb.DTOs;
using SmartCurrencyAssistantWeb.Services.Interfaces;

namespace SmartCurrencyAssistantWeb.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> AnalyzeTrendAsync(
            CurrencyTrendDto trend,
            CancellationToken cancellationToken = default)
        {
            var apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Gemini API key is not configured.");
            }

            var prompt = $"""
                You are a currency analysis assistant.

                Analyze the following historical exchange rate
                statistics and respond in Bulgarian.

                Explain the trend in simple language.
                Provide a cautious, practical recommendation.

                Use only the provided numerical data.
                Do not invent exchange rates or market events.
                Do not guarantee future exchange rate movements.
                Clearly state that this is not financial advice.

                Currency trend data:
                {JsonSerializer.Serialize(trend)}
                """;

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent");

            request.Headers.Add("x-goog-api-key", apiKey);

            request.Content = JsonContent.Create(new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            });

            using var response = await _httpClient.SendAsync(
                request, cancellationToken);

            response.EnsureSuccessStatusCode();

            using var json = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync(
                    cancellationToken),
                cancellationToken: cancellationToken);

            var analysis = json.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(analysis))
            {
                throw new InvalidOperationException(
                    "Gemini returned an empty analysis.");
            }

            return analysis;
        }
    }
}