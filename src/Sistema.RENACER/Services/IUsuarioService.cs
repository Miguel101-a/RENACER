using Sistema.RENACER.Models;

namespace Sistema.RENACER.Services;

public interface IUsuarioService
{
    Task<IEnumerable<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<(bool Exito, string? Error)> CrearAsync(string nombreUsuario, string password,
        string nombres, string apellidos, string? email, int idRol,
        int idUsuarioActual, string? ip);
    Task<(bool Exito, string? Error)> ActualizarAsync(int id, string nombres,
        string apellidos, string? email, int idRol, int idUsuarioActual, string? ip);
    Task<(bool Exito, string? Error)> CambiarPasswordAsync(int id, string nuevaPassword,
        int idUsuarioActual, string? ip);
    Task<(bool Exito, string? Error)> ToggleActivoAsync(int id, int idUsuarioActualLogueado, string? ip);
    Task<IEnumerable<Usuario>> ObtenerUltimosAccesosAsync(int top = 10);
}
