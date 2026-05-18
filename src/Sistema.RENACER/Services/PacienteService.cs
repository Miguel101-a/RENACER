using Sistema.RENACER.Models;
using Sistema.RENACER.Repositories;

namespace Sistema.RENACER.Services;

public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _repo;
    private readonly IAuditoriaService _auditoria;

    public PacienteService(IPacienteRepository repo, IAuditoriaService auditoria)
    {
        _repo = repo;
        _auditoria = auditoria;
    }

    public Task<IEnumerable<Paciente>> ObtenerTodosAsync(bool? soloActivos = null)
        => _repo.ObtenerTodosAsync(soloActivos);

    public Task<Paciente?> ObtenerPorIdAsync(int id)
        => _repo.ObtenerPorIdAsync(id);

    public async Task<(bool Exito, string? Error, int IdPaciente)> CrearAsync(
        Paciente paciente, int idUsuarioActual, string? ip)
    {
        if (!string.IsNullOrWhiteSpace(paciente.CI) &&
            await _repo.ExisteCIAsync(paciente.CI))
            return (false, "Ya existe un paciente con ese CI/documento.", 0);

        paciente.Activo = true;
        paciente.FechaRegistro = DateTime.Now;
        paciente.IdUsuarioRegistra = idUsuarioActual;

        await _repo.CrearAsync(paciente);

        await _auditoria.RegistrarAsync("Paciente", "INSERT", paciente.IdPaciente,
            null,
            new
            {
                paciente.Nombres, paciente.Apellidos, paciente.CI,
                paciente.FechaNacimiento, paciente.Genero, paciente.Telefono
            },
            idUsuarioActual, ip);

        return (true, null, paciente.IdPaciente);
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(
        Paciente paciente, int idUsuarioActual, string? ip)
    {
        var existente = await _repo.ObtenerPorIdAsync(paciente.IdPaciente);
        if (existente == null)
            return (false, "Paciente no encontrado.");

        if (!string.IsNullOrWhiteSpace(paciente.CI) &&
            await _repo.ExisteCIAsync(paciente.CI, paciente.IdPaciente))
            return (false, "Otro paciente ya usa ese CI/documento.");

        var anterior = new
        {
            existente.Nombres, existente.Apellidos, existente.CI,
            existente.Telefono, existente.Email, existente.MotivoConsulta
        };

        existente.Nombres         = paciente.Nombres;
        existente.Apellidos       = paciente.Apellidos;
        existente.FechaNacimiento = paciente.FechaNacimiento;
        existente.Genero          = paciente.Genero;
        existente.CI              = paciente.CI;
        existente.Telefono        = paciente.Telefono;
        existente.Email           = paciente.Email;
        existente.Direccion       = paciente.Direccion;
        existente.NombreTutor     = paciente.NombreTutor;
        existente.TelefonoTutor   = paciente.TelefonoTutor;
        existente.RelacionTutor   = paciente.RelacionTutor;
        existente.MotivoConsulta  = paciente.MotivoConsulta;

        await _repo.ActualizarAsync(existente);

        await _auditoria.RegistrarAsync("Paciente", "UPDATE", existente.IdPaciente,
            anterior,
            new
            {
                existente.Nombres, existente.Apellidos, existente.CI,
                existente.Telefono, existente.Email, existente.MotivoConsulta
            },
            idUsuarioActual, ip);

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> ToggleActivoAsync(
        int idPaciente, int idUsuarioActual, string? ip)
    {
        var paciente = await _repo.ObtenerPorIdAsync(idPaciente);
        if (paciente == null) return (false, "Paciente no encontrado.");

        var anterior = new { paciente.Activo };
        paciente.Activo = !paciente.Activo;

        await _repo.ActualizarAsync(paciente);

        await _auditoria.RegistrarAsync("Paciente", "UPDATE", paciente.IdPaciente,
            anterior, new { paciente.Activo },
            idUsuarioActual, ip);

        return (true, null);
    }
}
