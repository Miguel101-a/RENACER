using Sistema.RENACER.Models;

namespace Sistema.RENACER.Services;

public interface IRolService
{
    Task<IEnumerable<Rol>> ObtenerTodosAsync();
    Task<IEnumerable<Rol>> ObtenerActivosAsync();
    Task<Rol?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string? Error)> CrearAsync(string nombre, string? descripcion,
        int idUsuarioActual, string? ip);
    Task<(bool Exito, string? Error)> ActualizarAsync(int id, string nombre,
        string? descripcion, bool activo, int idUsuarioActual, string? ip);
    Task<(bool Exito, string? Error)> ActualizarPermisosAsync(int idRol,
        IEnumerable<int> idPermisos, int idUsuarioActual, string? ip);
}
