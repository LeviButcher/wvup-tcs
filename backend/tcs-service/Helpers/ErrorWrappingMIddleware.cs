#pragma warning disable 1591

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using tcs_service.Helpers;

public class ErrorWrappingMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorWrappingMiddleware> _logger;

    public ErrorWrappingMiddleware (RequestDelegate next, ILogger<ErrorWrappingMiddleware> logger) {
        _next = next;
        _logger = logger ??
            throw new ArgumentNullException (nameof (logger));
    }

    public async Task Invoke (HttpContext context) {
        try {
            await _next.Invoke (context);
        } catch (TCSException ex) {
            // _logger.LogError(EventIds.GlobalException, ex, ex.Message);

            context.Response.StatusCode = 500;

            context.Response.ContentType = "application/json";

            var response = new { message = ex.Message };

            var json = JsonConvert.SerializeObject (response);

            await context.Response.WriteAsync (json);
        }
    }
}