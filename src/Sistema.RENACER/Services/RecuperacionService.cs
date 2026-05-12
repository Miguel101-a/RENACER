using Microsoft.Extensions.Caching.Memory;
using Sistema.RENACER.Helpers;
using Sistema.RENACER.Repositories;

namespace Sistema.RENACER.Services;

public class RecuperacionService : IRecuperacionService
{
    private readonly IUsuarioRepository _repo;
    private readonly IAuditoriaService _auditoria;
    private readonly IMemoryCache _cache;
    private readonly int _minutosExpiracion;

    public RecuperacionService(IUsuarioRepository repo, IAuditoriaService auditoria,
        IMemoryCache cache, IConfiguration config)
    {
        _repo = repo;
        _auditoria = auditoria;
        _cache = cache;
        _minutosExpiracion = config.GetValue<int>("AppSettings:TokenRecuperacionMinutos", 30);
    }

    public async Task<(bool Exito, string? Error, string? Token)> GenerarTokenAsync(string nombreUsuario)
    {
        var usuario = await _repo.ObtenerPorNombreUsuarioAsync(nombreUsuario);
        if (usuario == null)
            return (false, "No se encontró un usuario con ese nombre.", null);

        if (!usuario.Activo)
            return (false, "Esta cuenta está deshabilitada.", null);

        var token = Guid.NewGuid().ToString("N").ToUpper();
        var key = $"recuperacion_{token}";

        _cache.Set(key, usuario.IdUsuario,
            TimeSpan.FromMinutes(_minutosExpiracion));

        return (true, null, token);
    }

    public async Task<(bool Exito, string? Error)> CambiarPasswordConTokenAsync(string token,
        string nuevaPassword, string? ip)
    {
        var key = $"recuperacion_{token.ToUpper()}";

        if (!_cache.TryGetValue(key, out int idUsuario))
            return (false, "El token es inválido o ha expirado.");

        await _repo.ActualizarPasswordHashAsync(idUsuario, PasswordHasher.HashBCrypt(nuevaPassword));

        await _auditoria.RegistrarAsync("Usuario", "UPDATE", idUsuario,
            null, new { Accion = "RecuperacionPassword" }, null, ip);

        _cache.Remove(key);

        return (true, null);
    }
}
