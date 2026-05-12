using Microsoft.EntityFrameworkCore;
using Sistema.RENACER.Data;
using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public class RolRepository : IRolRepository
{
    private readonly RenacerDbContext _db;

    public RolRepository(RenacerDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Rol>> ObtenerTodosAsync()
        => await _db.Roles
            .Include(r => r.RolPermisos).ThenInclude(rp => rp.Permiso)
            .OrderBy(r => r.Nombre)
            .ToListAsync();

    public async Task<IEnumerable<Rol>> ObtenerActivosAsync()
        => await _db.Roles
            .Where(r => r.Activo)
            .OrderBy(r => r.Nombre)
            .ToListAsync();

    public async Task<Rol?> ObtenerPorIdAsync(int id)
        => await _db.Roles
            .Include(r => r.RolPermisos).ThenInclude(rp => rp.Permiso)
            .FirstOrDefaultAsync(r => r.IdRol == id);

    public async Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null)
        => await _db.Roles.AnyAsync(r =>
            r.Nombre == nombre &&
            (excluirId == null || r.IdRol != excluirId));

    public async Task<Rol> CrearAsync(Rol rol)
    {
        _db.Roles.Add(rol);
        await _db.SaveChangesAsync();
        return rol;
    }

    public async Task<Rol> ActualizarAsync(Rol rol)
    {
        _db.Roles.Update(rol);
        await _db.SaveChangesAsync();
        return rol;
    }

    public async Task ActualizarPermisosAsync(int idRol, IEnumerable<int> idPermisos)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();
        var actuales = _db.RolPermisos.Where(rp => rp.IdRol == idRol);
        _db.RolPermisos.RemoveRange(actuales);
        await _db.SaveChangesAsync();

        foreach (var idPermiso in idPermisos)
        {
            _db.RolPermisos.Add(new RolPermiso { IdRol = idRol, IdPermiso = idPermiso });
        }
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
