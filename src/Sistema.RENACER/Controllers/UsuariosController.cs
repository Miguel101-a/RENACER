using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema.RENACER.Helpers;
using Sistema.RENACER.Services;
using Sistema.RENACER.ViewModels.Usuarios;

namespace Sistema.RENACER.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly IRolService _rolService;

    public UsuariosController(IUsuarioService usuarioService, IRolService rolService)
    {
        _usuarioService = usuarioService;
        _rolService = rolService;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _usuarioService.ObtenerTodosAsync();
        var vm = usuarios.Select(u => new UsuarioListaViewModel
        {
            IdUsuario = u.IdUsuario,
            NombreCompleto = u.NombreCompleto,
            NombreUsuario = u.NombreUsuario,
            Email = u.Email,
            Rol = u.UsuarioRoles.FirstOrDefault()?.Rol?.Nombre ?? "-",
            Activo = u.Activo,
            UltimoAcceso = u.UltimoAcceso,
            FechaCreacion = u.FechaCreacion
        });
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var roles = await _rolService.ObtenerActivosAsync();
        var vm = new UsuarioCrearViewModel
        {
            Roles = roles.Select(r => new SelectListItem(r.Nombre, r.IdRol.ToString()))
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(UsuarioCrearViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Roles = (await _rolService.ObtenerActivosAsync())
                .Select(r => new SelectListItem(r.Nombre, r.IdRol.ToString()));
            return View(model);
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _usuarioService.CrearAsync(
            model.NombreUsuario, model.Password,
            model.Nombres, model.Apellidos, model.Email,
            model.IdRol, idActual, ip);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            model.Roles = (await _rolService.ObtenerActivosAsync())
                .Select(r => new SelectListItem(r.Nombre, r.IdRol.ToString()));
            return View(model);
        }

        TempData["Toast"] = "success|Usuario creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var usuario = await _usuarioService.ObtenerPorIdAsync(id);
        if (usuario == null) return NotFound();

        var roles = await _rolService.ObtenerActivosAsync();
        var rolActual = usuario.UsuarioRoles.FirstOrDefault()?.Rol;

        var vm = new UsuarioEditarViewModel
        {
            IdUsuario = usuario.IdUsuario,
            Nombres = usuario.Nombres,
            Apellidos = usuario.Apellidos,
            Email = usuario.Email,
            IdRol = rolActual?.IdRol ?? 0,
            Activo = usuario.Activo,
            NombreUsuario = usuario.NombreUsuario,
            Roles = roles.Select(r => new SelectListItem(r.Nombre, r.IdRol.ToString()))
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(UsuarioEditarViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Roles = (await _rolService.ObtenerActivosAsync())
                .Select(r => new SelectListItem(r.Nombre, r.IdRol.ToString()));
            return View(model);
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _usuarioService.ActualizarAsync(
            model.IdUsuario, model.Nombres, model.Apellidos,
            model.Email, model.IdRol, idActual, ip);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            model.Roles = (await _rolService.ObtenerActivosAsync())
                .Select(r => new SelectListItem(r.Nombre, r.IdRol.ToString()));
            return View(model);
        }

        TempData["Toast"] = "success|Usuario actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> CambiarPassword(int id)
    {
        var usuario = await _usuarioService.ObtenerPorIdAsync(id);
        if (usuario == null) return NotFound();

        return View(new CambiarPasswordUsuarioViewModel
        {
            IdUsuario = usuario.IdUsuario,
            NombreUsuario = usuario.NombreUsuario
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarPassword(CambiarPasswordUsuarioViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _usuarioService.CambiarPasswordAsync(
            model.IdUsuario, model.NuevaPassword, idActual, ip);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            return View(model);
        }

        TempData["Toast"] = "success|Contraseña actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActivo(int id)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _usuarioService.ToggleActivoAsync(id, idActual, ip);

        if (!exito)
            TempData["Toast"] = $"error|{error}";
        else
            TempData["Toast"] = "success|Estado del usuario actualizado.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var usuario = await _usuarioService.ObtenerPorIdAsync(id);
        if (usuario == null) return NotFound();

        var vm = new UsuarioListaViewModel
        {
            IdUsuario = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto,
            NombreUsuario = usuario.NombreUsuario,
            Email = usuario.Email,
            Rol = usuario.UsuarioRoles.FirstOrDefault()?.Rol?.Nombre ?? "-",
            Activo = usuario.Activo,
            UltimoAcceso = usuario.UltimoAcceso,
            FechaCreacion = usuario.FechaCreacion
        };
        return View(vm);
    }
}
