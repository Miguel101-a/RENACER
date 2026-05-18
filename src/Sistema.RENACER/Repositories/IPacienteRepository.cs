using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public interface IPacienteRepository
{
    Task<IEnumerable<Paciente>> ObtenerTodosAsync(bool? soloActivos = null);
    Task<Paciente?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteCIAsync(string ci, int? excluirId = null);
    Task<Paciente> CrearAsync(Paciente paciente);
    Task<Paciente> ActualizarAsync(Paciente paciente);
}
