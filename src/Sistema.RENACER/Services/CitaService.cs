using Sistema.RENACER.Models;
using Sistema.RENACER.Repositories;

namespace Sistema.RENACER.Services;

public class CitaService : ICitaService
{
    private readonly ICitaRepository _repo;
    private readonly IAuditoriaService _auditoria;

    public CitaService(ICitaRepository repo, IAuditoriaService auditoria)
    {
        _repo = repo;
        _auditoria = auditoria;
    }

    public Task<IEnumerable<Cita>> ObtenerTodasAsync(DateTime? desde = null, DateTime? hasta = null,
        string? estado = null, int? idPaciente = null)
        => _repo.ObtenerTodasAsync(desde, hasta, estado, idPaciente);

    public Task<Cita?> ObtenerPorIdAsync(int id)
        => _repo.ObtenerPorIdAsync(id);

    public async Task<(bool Exito, string? Error, int IdCita)> CrearAsync(
        Cita cita, int? idServicio, int idUsuarioActual, string? ip)
    {
        if (cita.FechaHora < DateTime.Now.AddMinutes(-1))
            return (false, "La fecha y hora de la cita no puede estar en el pasado.", 0);

        if (cita.DuracionMinutos <= 0)
            return (false, "La duración de la cita debe ser mayor a 0 minutos.", 0);

        if (cita.IdProfesional.HasValue &&
            await _repo.HaySolapamientoAsync(cita.IdProfesional.Value, cita.FechaHora, cita.DuracionMinutos))
        {
            return (false, "La profesional ya tiene otra cita programada en ese horario.", 0);
        }

        cita.Estado = CitaEstado.Programada;
        cita.FechaRegistro = DateTime.Now;
        cita.IdUsuarioRegistra = idUsuarioActual;

        await _repo.CrearAsync(cita);

        // Si se seleccionó un servicio, lo registramos como ServicioRealizado
        // (relación cita ↔ servicio definida en BD por la tabla ServicioRealizado).
        if (idServicio.HasValue && idServicio.Value > 0)
        {
            await _repo.RegistrarServicioRealizadoAsync(new ServicioRealizado
            {
                IdCita = cita.IdCita,
                IdServicio = idServicio.Value,
                FechaRealizacion = cita.FechaHora.Date,
                IdUsuarioRegistra = idUsuarioActual
            });
        }

        await _auditoria.RegistrarAsync("Cita", "INSERT", cita.IdCita,
            null,
            new
            {
                cita.IdPaciente, cita.IdTipoCita, cita.IdProfesional,
                cita.FechaHora, cita.DuracionMinutos, IdServicio = idServicio
            },
            idUsuarioActual, ip);

        return (true, null, cita.IdCita);
    }

    public async Task<(bool Exito, string? Error)> CambiarEstadoAsync(
        int idCita, string nuevoEstado, int idUsuarioActual, string? ip)
    {
        if (!CitaEstado.Todos.Contains(nuevoEstado))
            return (false, "Estado de cita inválido.");

        var cita = await _repo.ObtenerPorIdAsync(idCita);
        if (cita == null) return (false, "Cita no encontrada.");

        if (cita.Estado == CitaEstado.Cancelada)
            return (false, "La cita ya está cancelada y no puede cambiar de estado.");

        var anterior = new { cita.Estado };
        cita.Estado = nuevoEstado;

        await _repo.ActualizarAsync(cita);

        await _auditoria.RegistrarAsync("Cita", "UPDATE", cita.IdCita,
            anterior, new { cita.Estado },
            idUsuarioActual, ip);

        return (true, null);
    }
}
