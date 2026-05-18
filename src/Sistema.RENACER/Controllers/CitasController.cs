using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema.RENACER.Helpers;
using Sistema.RENACER.Models;
using Sistema.RENACER.Services;
using Sistema.RENACER.ViewModels.Citas;

namespace Sistema.RENACER.Controllers;

[Authorize]
public class CitasController : Controller
{
    private readonly ICitaService _citaService;
    private readonly IPacienteService _pacienteService;
    private readonly ICatalogoService _catalogoService;

    public CitasController(ICitaService citaService, IPacienteService pacienteService,
        ICatalogoService catalogoService)
    {
        _citaService = citaService;
        _pacienteService = pacienteService;
        _catalogoService = catalogoService;
    }

    [HttpGet]
    [RequierePermiso("CITA_VER")]
    public async Task<IActionResult> Index(DateTime? desde = null, DateTime? hasta = null,
        string? estado = null, int? idPaciente = null)
    {
        DateTime? desdeFiltro = desde;
        DateTime? hastaFiltro = hasta?.AddDays(1).AddSeconds(-1);

        var citas = await _citaService.ObtenerTodasAsync(desdeFiltro, hastaFiltro, estado, idPaciente);
        var vm = citas.Select(c => new CitaListaViewModel
        {
            IdCita          = c.IdCita,
            Paciente        = c.Paciente?.NombreCompleto ?? "—",
            FechaHora       = c.FechaHora,
            DuracionMinutos = c.DuracionMinutos,
            TipoCita        = c.TipoCita?.Nombre ?? "—",
            Profesional     = c.Profesional?.NombreCompleto ?? "—",
            Estado          = c.Estado
        });

        ViewBag.FiltroDesde  = desde?.ToString("yyyy-MM-dd");
        ViewBag.FiltroHasta  = hasta?.ToString("yyyy-MM-dd");
        ViewBag.FiltroEstado = estado;
        ViewBag.Estados      = new SelectList(CitaEstado.Todos);
        return View(vm);
    }

    [HttpGet]
    [RequierePermiso("CITA_CREAR")]
    public async Task<IActionResult> Crear()
    {
        var vm = await ConstruirFormAsync(new CitaFormViewModel());
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequierePermiso("CITA_CREAR")]
    public async Task<IActionResult> Crear(CitaFormViewModel model)
    {
        if (model.HoraFin <= model.HoraInicio)
            ModelState.AddModelError(nameof(model.HoraFin),
                "La hora de fin debe ser posterior a la hora de inicio.");

        if (!ModelState.IsValid)
        {
            await ConstruirFormAsync(model);
            return View(model);
        }

        var cita = new Cita
        {
            IdPaciente      = model.IdPaciente,
            IdTipoCita      = model.IdTipoCita,
            IdProfesional   = model.IdProfesional,
            FechaHora       = model.FechaHora,
            DuracionMinutos = model.DuracionMinutos,
            Observaciones   = model.Observaciones
        };

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error, _) = await _citaService.CrearAsync(cita, model.IdServicio, idActual, ip);
        if (!exito)
        {
            ModelState.AddModelError(string.Empty, error!);
            await ConstruirFormAsync(model);
            return View(model);
        }

        TempData["Toast"] = "success|Cita agendada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequierePermiso("CITA_VER")]
    public async Task<IActionResult> Detalle(int id)
    {
        var c = await _citaService.ObtenerPorIdAsync(id);
        if (c == null) return NotFound();

        var vm = new CitaDetalleViewModel
        {
            IdCita          = c.IdCita,
            Paciente        = c.Paciente?.NombreCompleto ?? "—",
            FechaHora       = c.FechaHora,
            DuracionMinutos = c.DuracionMinutos,
            TipoCita        = c.TipoCita?.Nombre ?? "—",
            Profesional     = c.Profesional?.NombreCompleto ?? "—",
            Estado          = c.Estado,
            Observaciones   = c.Observaciones,
            FechaRegistro   = c.FechaRegistro
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
    {
        // CITA_CANCELAR para cancelar; CITA_EDITAR para los demás cambios de estado.
        var permiso = nuevoEstado == CitaEstado.Cancelada ? "CITA_CANCELAR" : "CITA_EDITAR";
        if (!ClaimsHelper.TienePermiso(User, permiso))
        {
            TempData["Toast"] = "error|No tiene permiso para esta acción.";
            return RedirectToAction(nameof(Index));
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var idActual = ClaimsHelper.GetIdUsuario(User);

        var (exito, error) = await _citaService.CambiarEstadoAsync(id, nuevoEstado, idActual, ip);

        TempData["Toast"] = exito
            ? $"success|Cita marcada como {nuevoEstado}."
            : $"error|{error}";

        return RedirectToAction(nameof(Index));
    }

    private async Task<CitaFormViewModel> ConstruirFormAsync(CitaFormViewModel vm)
    {
        var pacientes = await _pacienteService.ObtenerTodosAsync(soloActivos: true);
        var tipos     = await _catalogoService.ObtenerTiposCitaAsync();
        var servicios = await _catalogoService.ObtenerServiciosActivosAsync();
        var profes    = await _catalogoService.ObtenerProfesionalesActivosAsync();

        vm.Pacientes = pacientes.Select(p =>
            new SelectListItem($"{p.NombreCompleto}{(string.IsNullOrWhiteSpace(p.CI) ? "" : $" (CI {p.CI})")}",
                p.IdPaciente.ToString()));
        vm.TiposCita     = tipos.Select(t => new SelectListItem(t.Nombre, t.IdTipoCita.ToString()));
        vm.Servicios     = servicios.Select(s => new SelectListItem(s.Nombre, s.IdServicio.ToString()));
        vm.Profesionales = profes.Select(p =>
            new SelectListItem(p.NombreCompleto, p.IdProfesional.ToString()));

        return vm;
    }
}
