using Sistema.RENACER.Models;

namespace Sistema.RENACER.Services;

public interface ICitaService
{
    Task<IEnumerable<Cita>> ObtenerTodasAsync(DateTime? desde = null, DateTime? hasta = null,
        string? estado = null, int? idPaciente = null);

    Task<Cita?> ObtenerPorIdAsync(int id);

    Task<(bool Exito, string? Error, int IdCita)> CrearAsync(
        Cita cita, int? idServicio, int idUsuarioActual, string? ip);

    Task<(bool Exito, string? Error)> CambiarEstadoAsync(
        int idCita, string nuevoEstado, int idUsuarioActual, string? ip);
}
