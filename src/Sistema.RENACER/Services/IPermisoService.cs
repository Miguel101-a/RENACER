using Sistema.RENACER.Models;

namespace Sistema.RENACER.Services;

public interface IPermisoService
{
    Task<IEnumerable<Permiso>> ObtenerTodosAsync();
    Task<Permiso?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string? Error)> CrearAsync(string codigo, string nombre,
        string? descripcion, string categoria, int idUsuarioActual, string? ip);
    Task<(bool Exito, string? Error)> ActualizarAsync(int id, string nombre,
        string? descripcion, int idUsuarioActual, string? ip);
    Task<Dictionary<int, int>> ObtenerConteoRolesPorPermisoAsync();
}
