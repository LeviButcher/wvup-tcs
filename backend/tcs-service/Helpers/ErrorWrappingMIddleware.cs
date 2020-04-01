using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using tcs_service.Helpers;

///<summary>ErrorWrappingMiddleware</summary>
public class ErrorWrappingMiddleware
{
    private readonly RequestDelegate _next;

    ///<summary>ErrorWrappingMiddleware</summary>
    public ErrorWrappingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    ///<summary>ErrorWrappingMiddleware</summary>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (TCSException ex)
        {

            context.Response.StatusCode = 500;

            context.Response.ContentType = "application/json";

            var response = new { message = ex.Message };

            var json = JsonConvert.SerializeObject(response);

            await context.Response.WriteAsync(json);
        }
    }
}