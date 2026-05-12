using Sistema.RENACER.Models;
using Sistema.RENACER.Repositories;

namespace Sistema.RENACER.Services;

public class RolService : IRolService
{
    private static readonly HashSet<string> _rolesDelSistema =
        new(StringComparer.OrdinalIgnoreCase) { "Administrador", "Responsable" };

    private readonly IRolRepository _repo;
    private readonly IAuditoriaService _auditoria;

    public RolService(IRolRepository repo, IAuditoriaService auditoria)
    {
        _repo = repo;
        _auditoria = auditoria;
    }

    public Task<IEnumerable<Rol>> ObtenerTodosAsync() => _repo.ObtenerTodosAsync();
    public Task<IEnumerable<Rol>> ObtenerActivosAsync() => _repo.ObtenerActivosAsync();
    public Task<Rol?> ObtenerPorIdAsync(int id) => _repo.ObtenerPorIdAsync(id);

    public async Task<(bool Exito, string? Error)> CrearAsync(string nombre, string? descripcion,
        int idUsuarioActual, string? ip)
    {
        if (await _repo.ExisteNombreAsync(nombre))
            return (false, "Ya existe un rol con ese nombre.");

        var rol = new Rol { Nombre = nombre, Descripcion = descripcion, Activo = true };
        await _repo.CrearAsync(rol);

        await _auditoria.RegistrarAsync("Rol", "INSERT", rol.IdRol,
            null, new { rol.Nombre, rol.Descripcion }, idUsuarioActual, ip);

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(int id, string nombre,
        string? descripcion, bool activo, int idUsuarioActual, string? ip)
    {
        var rol = await _repo.ObtenerPorIdAsync(id);
        if (rol == null) return (false, "Rol no encontrado.");

        if (!activo && _rolesDelSistema.Contains(rol.Nombre))
            return (false, $"No se puede desactivar el rol '{rol.Nombre}' porque es del sistema.");

        if (await _repo.ExisteNombreAsync(nombre, id))
            return (false, "Ya existe un rol con ese nombre.");

        var anterior = new { rol.Nombre, rol.Descripcion, rol.Activo };
        rol.Nombre = nombre;
        rol.Descripcion = descripcion;
        rol.Activo = activo;

        await _repo.ActualizarAsync(rol);

        await _auditoria.RegistrarAsync("Rol", "UPDATE", id,
            anterior, new { nombre, descripcion, activo }, idUsuarioActual, ip);

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> ActualizarPermisosAsync(int idRol,
        IEnumerable<int> idPermisos, int idUsuarioActual, string? ip)
    {
        var rol = await _repo.ObtenerPorIdAsync(idRol);
        if (rol == null) return (false, "Rol no encontrado.");

        await _repo.ActualizarPermisosAsync(idRol, idPermisos);

        await _auditoria.RegistrarAsync("RolPermiso", "UPDATE", idRol,
            null, new { idRol, Permisos = idPermisos }, idUsuarioActual, ip);

        return (true, null);
    }
}
