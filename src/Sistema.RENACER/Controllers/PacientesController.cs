using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.RENACER.Helpers;
using Sistema.RENACER.Models;
using Sistema.RENACER.Services;
using Sistema.RENACER.ViewModels.Pacientes;

namespace Sistema.RENACER.Controllers;

[Authorize]
public class PacientesController : Controller
{
    private readonly IPacienteService _pacienteService;

    public PacientesController(IPacienteService pacienteService)
    {
        _pacienteService = pacienteService;
    }

    [HttpGet]
    [RequierePermiso("PAC_VER")]
    public async Task<IActionResult> Index(string? estado = null)
    {
        bool? soloActivos = estado switch
        {
            "activos" => true,
            "inactivos" => false,
            _ => null
        };

        var pacientes = await _pacienteService.ObtenerTodosAsync(soloActivos);
        var vm = pacientes.Select(p => new PacienteListaViewModel
        {
            IdPaciente     = p.IdPaciente,
            NombreCompleto = p.NombreCompleto,
            CI             = p.CI,
            Edad           = p.Edad,
            Telefono       = p.Telefono,
            Activo         = p.Activo,
            FechaRegistro  = p.FechaRegistro
        });

        ViewBag.FiltroEstado = estado ?? "todos";
        return View(vm);
    }

    [HttpGet]
    [RequierePermiso("PAC_CREAR")]
    public IActionResult Crear() => View(new PacienteFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequierePermiso("PAC_CREAR")]
    public async Task<IActionResult> Crear(PacienteFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var paciente = MapToModel(model);
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error, _) = await _pacienteService.CrearAsync(paciente, idActual, ip);
        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            return View(model);
        }

        TempData["Toast"] = "success|Paciente registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequierePermiso("PAC_EDITAR")]
    public async Task<IActionResult> Editar(int id)
    {
        var paciente = await _pacienteService.ObtenerPorIdAsync(id);
        if (paciente == null) return NotFound();

        var vm = new PacienteFormViewModel
        {
            IdPaciente      = paciente.IdPaciente,
            Nombres         = paciente.Nombres,
            Apellidos       = paciente.Apellidos,
            FechaNacimiento = paciente.FechaNacimiento,
            CI              = paciente.CI,
            Genero          = paciente.Genero ?? "Femenino",
            Telefono        = paciente.Telefono,
            Email           = paciente.Email,
            Direccion       = paciente.Direccion,
            NombreTutor     = paciente.NombreTutor,
            TelefonoTutor   = paciente.TelefonoTutor,
            RelacionTutor   = paciente.RelacionTutor,
            MotivoConsulta  = paciente.MotivoConsulta
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequierePermiso("PAC_EDITAR")]
    public async Task<IActionResult> Editar(PacienteFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var paciente = MapToModel(model);
        paciente.IdPaciente = model.IdPaciente;
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _pacienteService.ActualizarAsync(paciente, idActual, ip);
        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            return View(model);
        }

        TempData["Toast"] = "success|Paciente actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequierePermiso("PAC_VER")]
    public async Task<IActionResult> Detalle(int id)
    {
        var p = await _pacienteService.ObtenerPorIdAsync(id);
        if (p == null) return NotFound();

        var vm = new PacienteDetalleViewModel
        {
            IdPaciente      = p.IdPaciente,
            NombreCompleto  = p.NombreCompleto,
            FechaNacimiento = p.FechaNacimiento,
            Edad            = p.Edad,
            Genero          = p.Genero,
            CI              = p.CI,
            Telefono        = p.Telefono,
            Email           = p.Email,
            Direccion       = p.Direccion,
            NombreTutor     = p.NombreTutor,
            TelefonoTutor   = p.TelefonoTutor,
            RelacionTutor   = p.RelacionTutor,
            MotivoConsulta  = p.MotivoConsulta,
            Activo          = p.Activo,
            FechaRegistro   = p.FechaRegistro
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequierePermiso("PAC_ELIMINAR")]
    public async Task<IActionResult> ToggleActivo(int id)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _pacienteService.ToggleActivoAsync(id, idActual, ip);

        TempData["Toast"] = exito
            ? "success|Estado del paciente actualizado."
            : $"error|{error}";

        return RedirectToAction(nameof(Index));
    }

    private static Paciente MapToModel(PacienteFormViewModel m) => new()
    {
        Nombres         = m.Nombres,
        Apellidos       = m.Apellidos,
        FechaNacimiento = m.FechaNacimiento,
        Genero          = m.Genero,
        CI              = m.CI,
        Telefono        = m.Telefono,
        Email           = m.Email,
        Direccion       = m.Direccion,
        NombreTutor     = m.NombreTutor,
        TelefonoTutor   = m.TelefonoTutor,
        RelacionTutor   = m.RelacionTutor,
        MotivoConsulta  = m.MotivoConsulta
    };
}
