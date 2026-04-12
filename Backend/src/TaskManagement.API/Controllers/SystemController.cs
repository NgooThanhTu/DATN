using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TaskManagement.API.Filters;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [SystemAuthorize]
    public class SystemController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public SystemController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet("metrics")]
        public IActionResult GetMetrics()
        {
            if (_cache.TryGetValue("ApiPerformanceMetrics", out List<long>? metrics) && metrics != null)
            {
                return Ok(new { statusCode = 200, data = metrics });
            }
            return Ok(new { statusCode = 200, data = new List<long>() });
        }
    }
}
