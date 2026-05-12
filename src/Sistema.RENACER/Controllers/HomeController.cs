using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.RENACER.Services;
using Sistema.RENACER.ViewModels.Usuarios;

namespace Sistema.RENACER.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public HomeController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public async Task<IActionResult> Index()
    {
        var ultimos = await _usuarioService.ObtenerUltimosAccesosAsync(10);
        var vm = ultimos.Select(u => new UsuarioListaViewModel
        {
            IdUsuario = u.IdUsuario,
            NombreCompleto = u.NombreCompleto,
            NombreUsuario = u.NombreUsuario,
            UltimoAcceso = u.UltimoAcceso
        });
        return View(vm);
    }
}
