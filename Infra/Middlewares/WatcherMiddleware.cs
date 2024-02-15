using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Middlewares
{
    public  class WatcherMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<WatcherMiddleware> _logger;

        public WatcherMiddleware(RequestDelegate next,
            ILogger<WatcherMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();

            // Let the middleware pipeline run
            await _next(context);

            watcher.Stop();
            using (LogContext.PushProperty("Time", watcher.ElapsedMilliseconds))
            using (LogContext.PushProperty("LogType", "RESPONSE_TIME"))
            {
                _logger.LogInformation("Execution time");
            }
        }
    }
}
