using Microsoft.AspNetCore.Builder;

namespace tcs_service.Helpers
{
    ///<summary>MiddlerwareExtensions</summary>
    public static class MiddlewareExtensions
    {
        ///<summary>UseErrorWrapping</summary>
        public static IApplicationBuilder UseErrorWrapping(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorWrappingMiddleware>();
        }
    }
}