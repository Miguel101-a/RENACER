using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public interface IRolRepository
{
    Task<IEnumerable<Rol>> ObtenerTodosAsync();
    Task<IEnumerable<Rol>> ObtenerActivosAsync();
    Task<Rol?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null);
    Task<Rol> CrearAsync(Rol rol);
    Task<Rol> ActualizarAsync(Rol rol);
    Task ActualizarPermisosAsync(int idRol, IEnumerable<int> idPermisos);
}
