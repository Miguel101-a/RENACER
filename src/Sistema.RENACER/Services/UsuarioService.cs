using Sistema.RENACER.Helpers;
using Sistema.RENACER.Models;
using Sistema.RENACER.Repositories;

namespace Sistema.RENACER.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;
    private readonly IAuditoriaService _auditoria;

    public UsuarioService(IUsuarioRepository repo, IAuditoriaService auditoria)
    {
        _repo = repo;
        _auditoria = auditoria;
    }

    public Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        => _repo.ObtenerTodosAsync();

    public Task<Usuario?> ObtenerPorIdAsync(int id)
        => _repo.ObtenerPorIdAsync(id);

    public async Task<(bool Exito, string? Error)> CrearAsync(string nombreUsuario, string password,
        string nombres, string apellidos, string? email, int idRol,
        int idUsuarioActual, string? ip)
    {
        if (await _repo.ExisteNombreUsuarioAsync(nombreUsuario))
            return (false, "El nombre de usuario ya está en uso.");

        var usuario = new Usuario
        {
            NombreUsuario = nombreUsuario,
            PasswordHash = PasswordHasher.HashBCrypt(password),
            Nombres = nombres,
            Apellidos = apellidos,
            Email = email,
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        await _repo.CrearAsync(usuario, idRol);

        await _auditoria.RegistrarAsync("Usuario", "INSERT", usuario.IdUsuario,
            null, new { usuario.NombreUsuario, usuario.Nombres, usuario.Apellidos, idRol },
            idUsuarioActual, ip);

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> ActualizarAsync(int id, string nombres,
        string apellidos, string? email, int idRol, int idUsuarioActual, string? ip)
    {
        var usuario = await _repo.ObtenerPorIdAsync(id);
        if (usuario == null) return (false, "Usuario no encontrado.");

        var anterior = new { usuario.Nombres, usuario.Apellidos, usuario.Email };

        usuario.Nombres = nombres;
        usuario.Apellidos = apellidos;
        usuario.Email = email;

        await _repo.ActualizarAsync(usuario);
        await _repo.AsignarRolAsync(id, idRol);

        await _auditoria.RegistrarAsync("Usuario", "UPDATE", id,
            anterior, new { nombres, apellidos, email, idRol },
            idUsuarioActual, ip);

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> CambiarPasswordAsync(int id, string nuevaPassword,
        int idUsuarioActual, string? ip)
    {
        var usuario = await _repo.ObtenerPorIdAsync(id);
        if (usuario == null) return (false, "Usuario no encontrado.");

        await _repo.ActualizarPasswordHashAsync(id, PasswordHasher.HashBCrypt(nuevaPassword));

        await _auditoria.RegistrarAsync("Usuario", "UPDATE", id,
            null, new { Accion = "CambioPassword" },
            idUsuarioActual, ip);

        return (true, null);
    }

    public async Task<(bool Exito, string? Error)> ToggleActivoAsync(int id, int idUsuarioActualLogueado, string? ip)
    {
        if (id == idUsuarioActualLogueado)
            return (false, "No puede desactivar su propia cuenta.");

        var usuario = await _repo.ObtenerPorIdAsync(id);
        if (usuario == null) return (false, "Usuario no encontrado.");

        var anterior = new { usuario.Activo };
        usuario.Activo = !usuario.Activo;
        await _repo.ActualizarAsync(usuario);

        await _auditoria.RegistrarAsync("Usuario", "UPDATE", id,
            anterior, new { usuario.Activo },
            idUsuarioActualLogueado, ip);

        return (true, null);
    }

    public Task<IEnumerable<Usuario>> ObtenerUltimosAccesosAsync(int top = 10)
        => _repo.ObtenerUltimosAccesosAsync(top);
}
