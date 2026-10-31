
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SmartCurrencyAssistantWeb.Services
{
    public class ApiExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (!httpContext.Request.Path.StartsWithSegments("/api"))
            {
                return false;
            }

            int statusCode;
            string title;
            string detail;

            if (exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                title = "Invalid request";
                detail = exception.Message;
            }

            else if (exception is GeminiApiException geminiException)
            {
                if (geminiException.InnerException is OperationCanceledException)
                {
                    statusCode = StatusCodes.Status504GatewayTimeout;
                    title = "AI service timeout";
                    detail = "The AI service did not respond in time.";
                }
                else if (geminiException.InnerException is HttpRequestException httpError &&
                         httpError.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    statusCode = StatusCodes.Status503ServiceUnavailable;
                    title = "AI service unavailable";
                    detail = "The AI service is temporarily unavailable. Please try again later.";
                }
                else
                {
                    statusCode = StatusCodes.Status502BadGateway;
                    title = "AI service error";
                    detail = "Unable to retrieve an analysis from the AI service.";
                }
            }

            else if (exception is HttpRequestException httpException)
            {
                if (httpException.StatusCode is
                    HttpStatusCode.BadRequest or
                    HttpStatusCode.NotFound or
                    HttpStatusCode.UnprocessableEntity)
                {
                    statusCode = StatusCodes.Status422UnprocessableEntity;
                    title = "Exchange rate unavailable";
                    detail = "The exchange rate provider cannot process the requested currencies or period.";
                }
                else
                {
                    statusCode = StatusCodes.Status502BadGateway;
                    title = "Exchange rate provider error";
                    detail = "Unable to retrieve data from the exchange rate provider.";
                }
            }
            else if (exception is OperationCanceledException &&
                !httpContext.RequestAborted.IsCancellationRequested)
            {
                statusCode = StatusCodes.Status504GatewayTimeout;
                title = "Exchange rate provider timeout";
                detail = "The exchange rate provider did not respond in time.";
            }
            else
            {
                return false;
            }

            httpContext.Response.StatusCode = statusCode;

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            };

            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken);

            return true;
        }
    }
}