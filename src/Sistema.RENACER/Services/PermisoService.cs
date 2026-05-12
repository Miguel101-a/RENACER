using Sistema.RENACER.Models;
using Sistema.RENACER.Repositories;

namespace Sistema.RENACER.Services;

public class PermisoService : IPermisoService
{
    private readonly IPermisoRepository _repo;
    private readonly IAuditoriaService _auditoria;

    public PermisoService(IPermisoRepository repo, IAuditoriaService auditoria)
    {
        _repo = repo;
        _auditoria = auditoria;
    }

    public Task<IEnumerable<Permiso>> ObtenerTodosAsync() => _repo.ObtenerTodosAsync();
    public Task<Permiso?> ObtenerPorIdAsync(int id) => _repo.ObtenerPorIdAsync(id);
    public Task<Dictionary<int, int>> ObtenerConteoRolesPorPermisoAsync() => _repo.ObtenerConteoRolesPorPermisoAsync();

    public async Task<(bool Exito, string? Error)> CrearAsync(string codigo, string nombre,
        string? descripcion, string categoria, int idUsuarioActual, string? ip)
    {
        codigo = codigo.ToUpper().Replace(" ", "_");

        if (await _repo.ExisteCodigoAsync(codigo))
            return (false, "Ya existe un permiso con ese código.");

        var permiso = new Permiso
        {
            Codigo = codigo,
            Nombre = nombre,
            Descripcion = descripcion,
            Categoria = categoria
        };
        await _repo.CrearAsync(permiso);

        await _auditoria.RegistrarAsync("Permiso", "INSERT", permiso.IdPermiso,
            null, new { permiso.Codigo, permiso.Nombre, permiso.Categoria },
            idUsuarioActual, ip);

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(int id, string nombre,
        string? descripcion, int idUsuarioActual, string? ip)
    {
        var permiso = await _repo.ObtenerPorIdAsync(id);
        if (permiso == null) return (false, "Permiso no encontrado.");

        var anterior = new { permiso.Nombre, permiso.Descripcion };
        permiso.Nombre = nombre;
        permiso.Descripcion = descripcion;

        await _repo.ActualizarAsync(permiso);

        await _auditoria.RegistrarAsync("Permiso", "UPDATE", id,
            anterior, new { nombre, descripcion }, idUsuarioActual, ip);

        return (true, null);
    }
}
