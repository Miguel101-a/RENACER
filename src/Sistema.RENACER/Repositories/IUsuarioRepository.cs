using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
    Task<IEnumerable<Usuario>> ObtenerTodosAsync();
    Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? excluirId = null);
    Task<Usuario> CrearAsync(Usuario usuario, int idRol);
    Task<Usuario> ActualizarAsync(Usuario usuario);
    Task ActualizarPasswordHashAsync(int idUsuario, string nuevoHash);
    Task ActualizarUltimoAccesoAsync(int idUsuario);
    Task<IEnumerable<Rol>> ObtenerRolesDeUsuarioAsync(int idUsuario);
    Task AsignarRolAsync(int idUsuario, int idRol);
    Task<IEnumerable<Usuario>> ObtenerUltimosAccesosAsync(int top = 10);
}
