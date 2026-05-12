using Microsoft.EntityFrameworkCore;
using Sistema.RENACER.Data;
using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public class PermisoRepository : IPermisoRepository
{
    private readonly RenacerDbContext _db;

    public PermisoRepository(RenacerDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Permiso>> ObtenerTodosAsync()
        => await _db.Permisos.OrderBy(p => p.Categoria).ThenBy(p => p.Nombre).ToListAsync();

    public async Task<Permiso?> ObtenerPorIdAsync(int id)
        => await _db.Permisos.FindAsync(id);

    public async Task<bool> ExisteCodigoAsync(string codigo, int? excluirId = null)
        => await _db.Permisos.AnyAsync(p =>
            p.Codigo == codigo &&
            (excluirId == null || p.IdPermiso != excluirId));

    public async Task<Permiso> CrearAsync(Permiso permiso)
    {
        _db.Permisos.Add(permiso);
        await _db.SaveChangesAsync();
        return permiso;
    }

    public async Task<Permiso> ActualizarAsync(Permiso permiso)
    {
        _db.Permisos.Update(permiso);
        await _db.SaveChangesAsync();
        return permiso;
    }

    public async Task<Dictionary<int, int>> ObtenerConteoRolesPorPermisoAsync()
        => await _db.RolPermisos
            .GroupBy(rp => rp.IdPermiso)
            .Select(g => new { IdPermiso = g.Key, Conteo = g.Count() })
            .ToDictionaryAsync(x => x.IdPermiso, x => x.Conteo);
}
