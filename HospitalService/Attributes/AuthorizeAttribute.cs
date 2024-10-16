using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HospitalService.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string[] Roles { get; set; }

    public AuthorizeAttribute(params string[] roles)
    {
        Roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Items["Roles"] is List<string> userRoles)
        {
            if (userRoles.Intersect(Roles).Count() == 0 && Roles.Length != 0)
            {
                context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
        else
        {
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
