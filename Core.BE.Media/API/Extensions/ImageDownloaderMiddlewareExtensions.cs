using Emeint.Core.BE.Media.API.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Emeint.Core.BE.Media.API.Extensions
{
    public static class ImageDownloaderMiddlewareExtensions
    {
        public static IServiceCollection AddImageDownloader(this IServiceCollection services)
        {
            return services.AddMemoryCache();
        }

        public static IApplicationBuilder UseImageDownloader(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImageDownloaderMiddleware>();
        }
    }
}
