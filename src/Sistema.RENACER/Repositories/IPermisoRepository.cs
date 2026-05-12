using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public interface IPermisoRepository
{
    Task<IEnumerable<Permiso>> ObtenerTodosAsync();
    Task<Permiso?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteCodigoAsync(string codigo, int? excluirId = null);
    Task<Permiso> CrearAsync(Permiso permiso);
    Task<Permiso> ActualizarAsync(Permiso permiso);
    Task<Dictionary<int, int>> ObtenerConteoRolesPorPermisoAsync();
}
