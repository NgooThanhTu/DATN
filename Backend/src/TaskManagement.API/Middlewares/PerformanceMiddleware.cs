using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskManagement.API.Middlewares
{
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private const string MetricCacheKey = "ApiPerformanceMetrics";

        public PerformanceMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            
            await _next(context);
            
            sw.Stop();
            
            if (context.Request.Path.Value != null && context.Request.Path.Value.StartsWith("/api"))
            {
                try
                {
                    var responseTime = sw.ElapsedMilliseconds;
                    var metrics = _cache.GetOrCreate(MetricCacheKey, entry => {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                        return new List<long>();
                    });

                    if (metrics != null)
                    {
                        lock (metrics)
                        {
                            metrics.Add(responseTime);
                            if (metrics.Count > 100) metrics.RemoveAt(0);
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignore cache exceptions to not crash the request
                }
            }
        }
    }
}
