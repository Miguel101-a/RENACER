using Sistema.RENACER.Models;

namespace Sistema.RENACER.Services;

public interface IPacienteService
{
    Task<IEnumerable<Paciente>> ObtenerTodosAsync(bool? soloActivos = null);
    Task<Paciente?> ObtenerPorIdAsync(int id);

    Task<(bool Exito, string? Error, int IdPaciente)> CrearAsync(
        Paciente paciente, int idUsuarioActual, string? ip);

    Task<(bool Exito, string? Error)> ActualizarAsync(
        Paciente paciente, int idUsuarioActual, string? ip);

    Task<(bool Exito, string? Error)> ToggleActivoAsync(
        int idPaciente, int idUsuarioActual, string? ip);
}
