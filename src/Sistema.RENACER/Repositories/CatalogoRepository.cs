using Microsoft.EntityFrameworkCore;
using Sistema.RENACER.Data;
using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public class CatalogoRepository : ICatalogoRepository
{
    private readonly RenacerDbContext _db;

    public CatalogoRepository(RenacerDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TipoCita>> ObtenerTiposCitaAsync()
        => await _db.TiposCita.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync();

    public async Task<IEnumerable<Servicio>> ObtenerServiciosActivosAsync()
        => await _db.Servicios.AsNoTracking()
            .Where(s => s.Activo)
            .OrderBy(s => s.Nombre)
            .ToListAsync();

    public async Task<IEnumerable<ProfesionalExterno>> ObtenerProfesionalesActivosAsync()
        => await _db.ProfesionalesExternos.AsNoTracking()
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombres).ThenBy(p => p.Apellidos)
            .ToListAsync();
}
