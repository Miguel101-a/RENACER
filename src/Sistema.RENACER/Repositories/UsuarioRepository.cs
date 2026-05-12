using Microsoft.EntityFrameworkCore;
using Sistema.RENACER.Data;
using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly RenacerDbContext _db;

    public UsuarioRepository(RenacerDbContext db)
    {
        _db = db;
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
        => await _db.Usuarios
            .Include(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

    public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        => await _db.Usuarios
            .Include(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

    public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        => await _db.Usuarios
            .Include(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .OrderBy(u => u.Apellidos).ThenBy(u => u.Nombres)
            .ToListAsync();

    public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? excluirId = null)
        => await _db.Usuarios.AnyAsync(u =>
            u.NombreUsuario == nombreUsuario &&
            (excluirId == null || u.IdUsuario != excluirId));

    public async Task<Usuario> CrearAsync(Usuario usuario, int idRol)
    {
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        _db.UsuarioRoles.Add(new UsuarioRol { IdUsuario = usuario.IdUsuario, IdRol = idRol });
        await _db.SaveChangesAsync();

        return usuario;
    }

    public async Task<Usuario> ActualizarAsync(Usuario usuario)
    {
        _db.Usuarios.Update(usuario);
        await _db.SaveChangesAsync();
        return usuario;
    }

    public async Task ActualizarPasswordHashAsync(int idUsuario, string nuevoHash)
    {
        await _db.Database.ExecuteSqlRawAsync(
            "EXEC sp_ActualizarPasswordHash @IdUsuario = {0}, @NuevoHash = {1}",
            idUsuario, nuevoHash);
    }

    public async Task ActualizarUltimoAccesoAsync(int idUsuario)
    {
        await _db.Database.ExecuteSqlRawAsync(
            "EXEC sp_RegistrarUltimoAcceso @IdUsuario = {0}", idUsuario);
    }

    public async Task<IEnumerable<Rol>> ObtenerRolesDeUsuarioAsync(int idUsuario)
        => await _db.UsuarioRoles
            .Where(ur => ur.IdUsuario == idUsuario)
            .Select(ur => ur.Rol)
            .ToListAsync();

    public async Task AsignarRolAsync(int idUsuario, int idRol)
    {
        var existente = await _db.UsuarioRoles
            .FirstOrDefaultAsync(ur => ur.IdUsuario == idUsuario);
        if (existente != null)
        {
            _db.UsuarioRoles.Remove(existente);
        }
        _db.UsuarioRoles.Add(new UsuarioRol { IdUsuario = idUsuario, IdRol = idRol });
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Usuario>> ObtenerUltimosAccesosAsync(int top = 10)
        => await _db.Usuarios
            .Where(u => u.UltimoAcceso != null)
            .OrderByDescending(u => u.UltimoAcceso)
            .Take(top)
            .ToListAsync();
}
