using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.RENACER.Helpers;
using Sistema.RENACER.Repositories;
using Sistema.RENACER.Services;
using Sistema.RENACER.ViewModels.Cuenta;

namespace Sistema.RENACER.Controllers;

public class CuentaController : Controller
{
    private readonly IAuthService _auth;
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IRolRepository _rolRepo;
    private readonly IPermisoRepository _permisoRepo;
    private readonly IRecuperacionService _recuperacion;

    public CuentaController(IAuthService auth, IUsuarioRepository usuarioRepo,
        IRolRepository rolRepo, IPermisoRepository permisoRepo,
        IRecuperacionService recuperacion)
    {
        _auth = auth;
        _usuarioRepo = usuarioRepo;
        _rolRepo = rolRepo;
        _permisoRepo = permisoRepo;
        _recuperacion = recuperacion;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var resultado = await _auth.AutenticarAsync(model.NombreUsuario, model.Password, ip);

        if (!resultado.Exito)
        {
            ModelState.AddModelError(string.Empty, resultado.MensajeError!);
            return View(model);
        }

        var usuario = resultado.Usuario!;

        // Cargar roles y permisos para los claims
        var roles = await _rolRepo.ObtenerActivosAsync();
        var rolUsuario = usuario.UsuarioRoles.FirstOrDefault()?.Rol;

        var permisos = new List<string>();
        if (rolUsuario != null)
        {
            var rolConPermisos = await _rolRepo.ObtenerPorIdAsync(rolUsuario.IdRol);
            if (rolConPermisos != null)
                permisos = rolConPermisos.RolPermisos.Select(rp => rp.Permiso.Codigo).ToList();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new(ClaimTypes.Name, usuario.NombreUsuario),
            new("Nombres", $"{usuario.Nombres} {usuario.Apellidos}".Trim()),
            new(ClaimTypes.Email, usuario.Email ?? string.Empty),
            new(ClaimTypes.Role, rolUsuario?.Nombre ?? string.Empty),
            new("Permisos", string.Join(",", permisos)),
            new("IdRol", rolUsuario?.IdRol.ToString() ?? "0")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = model.Recordarme,
            ExpiresUtc = model.Recordarme
                ? DateTimeOffset.UtcNow.AddDays(30)
                : DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            principal, authProps);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RecuperarPassword()
    {
        return View(new RecuperarPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecuperarPassword(RecuperarPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var (exito, error, token) = await _recuperacion.GenerarTokenAsync(model.NombreUsuario);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            return View(model);
        }

        // En producción el token iría por email. Aquí se muestra en pantalla
        // para que el Administrador lo entregue manualmente al usuario.
        TempData["TokenGenerado"] = token;
        TempData["UsuarioRecuperacion"] = model.NombreUsuario;

        return RedirectToAction("CambiarPassword");
    }

    [HttpGet]
    public IActionResult CambiarPassword()
    {
        var token = TempData["TokenGenerado"]?.ToString();
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("RecuperarPassword");

        ViewData["UsuarioRecuperacion"] = TempData["UsuarioRecuperacion"];
        return View(new CambiarPasswordViewModel { Token = token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var (exito, error) = await _recuperacion.CambiarPasswordConTokenAsync(model.Token, model.NuevaPassword, ip);

        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            return View(model);
        }

        TempData["Mensaje"] = "Contraseña actualizada correctamente. Puede iniciar sesión.";
        return RedirectToAction("Login");
    }
}
