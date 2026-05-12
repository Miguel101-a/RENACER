using System.Security.Claims;

namespace Sistema.RENACER.Helpers;

public static class ClaimsHelper
{
    public static int GetIdUsuario(ClaimsPrincipal user)
    {
        var val = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(val, out var id) ? id : 0;
    }

    public static string GetNombreUsuario(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public static string GetNombreCompleto(ClaimsPrincipal user)
        => user.FindFirstValue("Nombres") ?? string.Empty;

    public static string GetRol(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    public static bool TienePermiso(ClaimsPrincipal user, string codigoPermiso)
    {
        var permisos = user.FindFirstValue("Permisos") ?? string.Empty;
        return permisos.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Any(p => p.Trim().Equals(codigoPermiso, StringComparison.OrdinalIgnoreCase));
    }
}
