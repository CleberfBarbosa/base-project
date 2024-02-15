using Infra.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Infra.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseDefaultMiddewares(this WebApplication application)
        {
            application.UseSerilogEnrichMiddleware();
            application.UseErrorMiddleware();
            application.UseWatchMiddleware();
        }

        public static void UseErrorMiddleware(this WebApplication @this)
        {
            @this.UseMiddleware<ErrorMiddleware>();
        }

        public static void UseSerilogEnrichMiddleware(this WebApplication @this)
        {
            @this.UseMiddleware<SerilogEnrichMiddleware>();
        }

        public static void UseWatchMiddleware(this WebApplication @this)
        {
            @this.UseMiddleware<WatcherMiddleware>();
        }
    }
}
