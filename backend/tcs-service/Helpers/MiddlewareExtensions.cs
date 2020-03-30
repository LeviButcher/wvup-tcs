#pragma warning disable 1591

using Microsoft.AspNetCore.Builder;

namespace Helpers {
    public static class MiddlewareExtensions {
        public static IApplicationBuilder UseErrorWrapping (
            this IApplicationBuilder builder) {
            return builder.UseMiddleware<ErrorWrappingMiddleware> ();
        }
    }
}