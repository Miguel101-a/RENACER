using Microsoft.EntityFrameworkCore;
using Sistema.RENACER.Data;
using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private readonly RenacerDbContext _db;

    public PacienteRepository(RenacerDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Paciente>> ObtenerTodosAsync(bool? soloActivos = null)
    {
        var query = _db.Pacientes.AsNoTracking().AsQueryable();
        if (soloActivos.HasValue)
            query = query.Where(p => p.Activo == soloActivos.Value);

        return await query
            .OrderBy(p => p.Apellidos).ThenBy(p => p.Nombres)
            .ToListAsync();
    }

    public async Task<Paciente?> ObtenerPorIdAsync(int id)
        => await _db.Pacientes.FirstOrDefaultAsync(p => p.IdPaciente == id);

    public async Task<bool> ExisteCIAsync(string ci, int? excluirId = null)
        => await _db.Pacientes.AnyAsync(p =>
            p.CI == ci &&
            (excluirId == null || p.IdPaciente != excluirId));

    public async Task<Paciente> CrearAsync(Paciente paciente)
    {
        _db.Pacientes.Add(paciente);
        await _db.SaveChangesAsync();
        return paciente;
    }

    public async Task<Paciente> ActualizarAsync(Paciente paciente)
    {
        _db.Pacientes.Update(paciente);
        await _db.SaveChangesAsync();
        return paciente;
    }
}
