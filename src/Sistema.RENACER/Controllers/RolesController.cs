using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.RENACER.Helpers;
using Sistema.RENACER.Services;
using Sistema.RENACER.ViewModels.Roles;

namespace Sistema.RENACER.Controllers;

[Authorize(Roles = "Administrador")]
public class RolesController : Controller
{
    private readonly IRolService _rolService;
    private readonly IPermisoService _permisoService;

    public RolesController(IRolService rolService, IPermisoService permisoService)
    {
        _rolService = rolService;
        _permisoService = permisoService;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _rolService.ObtenerTodosAsync();
        var vm = roles.Select(r => new RolViewModel
        {
            IdRol = r.IdRol,
            Nombre = r.Nombre,
            Descripcion = r.Descripcion,
            Activo = r.Activo,
            CodigosPermisos = r.RolPermisos.Select(rp => rp.Permiso.Codigo).ToList()
        });
        return View(vm);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View(new RolViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(RolViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _rolService.CrearAsync(model.Nombre, model.Descripcion, idActual, ip);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            return View(model);
        }

        TempData["Toast"] = "success|Rol creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var rol = await _rolService.ObtenerPorIdAsync(id);
        if (rol == null) return NotFound();

        return View(new RolViewModel
        {
            IdRol = rol.IdRol,
            Nombre = rol.Nombre,
            Descripcion = rol.Descripcion,
            Activo = rol.Activo
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(RolViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _rolService.ActualizarAsync(
            model.IdRol, model.Nombre, model.Descripcion, model.Activo, idActual, ip);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            return View(model);
        }

        TempData["Toast"] = "success|Rol actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> AsignarPermisos(int id)
    {
        var rol = await _rolService.ObtenerPorIdAsync(id);
        if (rol == null) return NotFound();

        var todosPermisos = await _permisoService.ObtenerTodosAsync();
        var permisosAsignados = rol.RolPermisos.Select(rp => rp.IdPermiso).ToHashSet();

        var vm = new AsignarPermisosViewModel
        {
            IdRol = rol.IdRol,
            NombreRol = rol.Nombre,
            Permisos = todosPermisos.Select(p => new PermisoCheckItem
            {
                IdPermiso = p.IdPermiso,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Categoria = p.Categoria,
                Asignado = permisosAsignados.Contains(p.IdPermiso)
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AsignarPermisos(AsignarPermisosViewModel model)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var seleccionados = model.PermisosSeleccionados ?? new List<int>();

        var (exito, error) = await _rolService.ActualizarPermisosAsync(
            model.IdRol, seleccionados, idActual, ip);

        if (!exito)
        {
            TempData["Toast"] = $"error|{error}";
        }
        else
        {
            TempData["Toast"] = "success|Permisos del rol actualizados.";
        }

        return RedirectToAction(nameof(Index));
    }
}
