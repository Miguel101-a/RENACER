using Microsoft.EntityFrameworkCore;
using Sistema.RENACER.Data;
using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public class CitaRepository : ICitaRepository
{
    private readonly RenacerDbContext _db;

    public CitaRepository(RenacerDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Cita>> ObtenerTodasAsync(DateTime? desde = null, DateTime? hasta = null,
        string? estado = null, int? idPaciente = null)
    {
        var query = _db.Citas
            .Include(c => c.Paciente)
            .Include(c => c.TipoCita)
            .Include(c => c.Profesional)
            .AsNoTracking()
            .AsQueryable();

        if (desde.HasValue)      query = query.Where(c => c.FechaHora >= desde.Value);
        if (hasta.HasValue)      query = query.Where(c => c.FechaHora <= hasta.Value);
        if (!string.IsNullOrWhiteSpace(estado)) query = query.Where(c => c.Estado == estado);
        if (idPaciente.HasValue) query = query.Where(c => c.IdPaciente == idPaciente.Value);

        return await query.OrderBy(c => c.FechaHora).ToListAsync();
    }

    public async Task<Cita?> ObtenerPorIdAsync(int id)
        => await _db.Citas
            .Include(c => c.Paciente)
            .Include(c => c.TipoCita)
            .Include(c => c.Profesional)
            .FirstOrDefaultAsync(c => c.IdCita == id);

    public async Task<bool> HaySolapamientoAsync(int idProfesional, DateTime fechaHora, int duracionMinutos,
        int? excluirIdCita = null)
    {
        var fin = fechaHora.AddMinutes(duracionMinutos);

        // Misma profesional, citas no canceladas/reagendadas que se solapan
        return await _db.Citas.AnyAsync(c =>
            c.IdProfesional == idProfesional &&
            c.Estado != CitaEstado.Cancelada &&
            c.Estado != CitaEstado.Reagendada &&
            (excluirIdCita == null || c.IdCita != excluirIdCita) &&
            fechaHora < c.FechaHora.AddMinutes(c.DuracionMinutos) &&
            fin > c.FechaHora);
    }

    public async Task<Cita> CrearAsync(Cita cita)
    {
        _db.Citas.Add(cita);
        await _db.SaveChangesAsync();
        return cita;
    }

    public async Task<Cita> ActualizarAsync(Cita cita)
    {
        _db.Citas.Update(cita);
        await _db.SaveChangesAsync();
        return cita;
    }

    public async Task RegistrarServicioRealizadoAsync(ServicioRealizado servicioRealizado)
    {
        _db.ServiciosRealizados.Add(servicioRealizado);
        await _db.SaveChangesAsync();
    }
}
