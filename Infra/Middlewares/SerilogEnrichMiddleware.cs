using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Infra.Middlewares
{
    public class SerilogEnrichMiddleware
    {
        private readonly RequestDelegate _next;

        public SerilogEnrichMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("IpRequest", CurrentIp(context)))
            {
                return _next.Invoke(context);
            }
        }

        public static string CurrentIp(HttpContext httpContext)
        {
            var ip = httpContext.Request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(ip))
            {
                var ips = ip.ToString().Split(",");
                return ips[ips.Length - 1];
            }
            else
                return httpContext.Connection?.RemoteIpAddress?.ToString() ?? "";
        }
    }
}
