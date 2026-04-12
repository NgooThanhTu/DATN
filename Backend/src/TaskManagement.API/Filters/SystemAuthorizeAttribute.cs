using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SystemAuthorizeAttribute : TypeFilterAttribute
    {
        public SystemAuthorizeAttribute(string roles = "SuperAdmin, Admin") : base(typeof(SystemAuthorizeFilter))
        {
            Arguments = new object[] { roles };
        }
    }

    public class SystemAuthorizeFilter : IAsyncActionFilter
    {
        private readonly string[] _allowedRoles;
        private readonly ApplicationDbContext _dbContext;

        public SystemAuthorizeFilter(string roles, ApplicationDbContext dbContext)
        {
            _allowedRoles = roles.Split(',').Select(r => r.Trim()).ToArray();
            _dbContext = dbContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userIdString = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                context.Result = new UnauthorizedObjectResult(new { statusCode = 401, message = "Unauthorized. JWT is missing or invalid." });
                return;
            }

            var user = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || !user.IsActive || user.IsDeleted)
            {
                context.Result = new ObjectResult(new { statusCode = 403, message = "Forbidden. Your account is suspended or deleted." })
                {
                    StatusCode = 403
                };
                return;
            }

            bool hasRole = user.UserRoles.Any(ur => ur.Role != null && _allowedRoles.Any(ar => ar.Equals(ur.Role.Name, StringComparison.OrdinalIgnoreCase)));

            if (!hasRole)
            {
                context.Result = new ObjectResult(new { statusCode = 403, message = "Forbidden. System access restricted to Admins only." })
                {
                    StatusCode = 403
                };
                return;
            }

            await next();
        }
    }
}
