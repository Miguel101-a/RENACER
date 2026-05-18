using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public interface ICitaRepository
{
    Task<IEnumerable<Cita>> ObtenerTodasAsync(DateTime? desde = null, DateTime? hasta = null,
        string? estado = null, int? idPaciente = null);
    Task<Cita?> ObtenerPorIdAsync(int id);
    Task<bool> HaySolapamientoAsync(int idProfesional, DateTime fechaHora, int duracionMinutos,
        int? excluirIdCita = null);
    Task<Cita> CrearAsync(Cita cita);
    Task<Cita> ActualizarAsync(Cita cita);
    Task RegistrarServicioRealizadoAsync(ServicioRealizado servicioRealizado);
}
