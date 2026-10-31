namespace SmartCurrencyAssistantWeb.Services
{
    public sealed class GeminiApiException : Exception
    {
        public GeminiApiException(Exception innerException) : base("Gemini API request failed",
            innerException)
        {

        }
    }
}