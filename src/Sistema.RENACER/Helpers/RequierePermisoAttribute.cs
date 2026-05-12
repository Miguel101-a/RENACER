using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sistema.RENACER.Helpers;

public class RequierePermisoAttribute : TypeFilterAttribute
{
    public RequierePermisoAttribute(string permiso)
        : base(typeof(RequierePermisoFilter))
    {
        Arguments = new object[] { permiso };
    }
}

public class RequierePermisoFilter : IAuthorizationFilter
{
    private readonly string _permiso;

    public RequierePermisoFilter(string permiso)
    {
        _permiso = permiso;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToActionResult("Login", "Cuenta", null);
            return;
        }

        if (!ClaimsHelper.TienePermiso(user, _permiso))
        {
            context.Result = new RedirectToActionResult("AccesoDenegado", "Cuenta", null);
        }
    }
}
