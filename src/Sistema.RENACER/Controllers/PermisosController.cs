using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.RENACER.Helpers;
using Sistema.RENACER.Services;
using Sistema.RENACER.ViewModels.Permisos;

namespace Sistema.RENACER.Controllers;

[Authorize(Roles = "Administrador")]
public class PermisosController : Controller
{
    private readonly IPermisoService _permisoService;

    public PermisosController(IPermisoService permisoService)
    {
        _permisoService = permisoService;
    }

    public async Task<IActionResult> Index()
    {
        var permisos = await _permisoService.ObtenerTodosAsync();
        var conteos = await _permisoService.ObtenerConteoRolesPorPermisoAsync();

        var vm = permisos.Select(p => new PermisoListaViewModel
        {
            IdPermiso = p.IdPermiso,
            Codigo = p.Codigo,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Categoria = p.Categoria,
            CantidadRoles = conteos.GetValueOrDefault(p.IdPermiso, 0)
        });
        return View(vm);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View(new PermisoCrearViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(PermisoCrearViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _permisoService.CrearAsync(
            model.Codigo, model.Nombre, model.Descripcion, model.Categoria, idActual, ip);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            return View(model);
        }

        TempData["Toast"] = "success|Permiso creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var permiso = await _permisoService.ObtenerPorIdAsync(id);
        if (permiso == null) return NotFound();

        return View(new PermisoEditarViewModel
        {
            IdPermiso = permiso.IdPermiso,
            Codigo = permiso.Codigo,
            Nombre = permiso.Nombre,
            Descripcion = permiso.Descripcion
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(PermisoEditarViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _permisoService.ActualizarAsync(
            model.IdPermiso, model.Nombre, model.Descripcion, idActual, ip);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            return View(model);
        }

        TempData["Toast"] = "success|Permiso actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
