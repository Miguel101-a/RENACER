using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Sistema.RENACER.Helpers;
using Sistema.RENACER.Repositories;

namespace Sistema.RENACER.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IMemoryCache _cache;
    private readonly int _maxIntentos;
    private readonly int _bloqueoMinutos;

    public AuthService(IUsuarioRepository usuarioRepo, IMemoryCache cache, IConfiguration config)
    {
        _usuarioRepo = usuarioRepo;
        _cache = cache;
        _maxIntentos = config.GetValue<int>("AppSettings:BloqueoIntentosFallidos", 5);
        _bloqueoMinutos = config.GetValue<int>("AppSettings:BloqueoMinutos", 15);
    }

    public async Task<LoginResultado> AutenticarAsync(string nombreUsuario, string password, string? ip)
    {
        if (EstaBloqueado(nombreUsuario))
            return new LoginResultado(false, $"Cuenta bloqueada temporalmente. Intente en {_bloqueoMinutos} minutos.", null);

        var usuario = await _usuarioRepo.ObtenerPorNombreUsuarioAsync(nombreUsuario);

        if (usuario == null || !PasswordHasher.Verify(password, usuario.PasswordHash))
        {
            RegistrarIntentoFallido(nombreUsuario);
            return new LoginResultado(false, "Usuario o contraseña incorrectos.", null);
        }

        if (!usuario.Activo)
            return new LoginResultado(false, "Esta cuenta está deshabilitada.", null);

        // Migración SHA-256 → BCrypt en el primer login exitoso
        if (!PasswordHasher.IsBCryptHash(usuario.PasswordHash))
        {
            var nuevoHash = PasswordHasher.HashBCrypt(password);
            await _usuarioRepo.ActualizarPasswordHashAsync(usuario.IdUsuario, nuevoHash);
        }

        await _usuarioRepo.ActualizarUltimoAccesoAsync(usuario.IdUsuario);
        LimpiarIntentos(nombreUsuario);

        return new LoginResultado(true, null, usuario);
    }

    public bool EstaBloqueado(string nombreUsuario)
    {
        var key = $"bloqueo_{nombreUsuario.ToLower()}";
        return _cache.TryGetValue(key, out _);
    }

    public void RegistrarIntentoFallido(string nombreUsuario)
    {
        var keyIntentos = $"intentos_{nombreUsuario.ToLower()}";
        var intentos = _cache.GetOrCreate(keyIntentos, e =>
        {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_bloqueoMinutos);
            return 0;
        });

        intentos++;
        _cache.Set(keyIntentos, intentos,
            TimeSpan.FromMinutes(_bloqueoMinutos));

        if (intentos >= _maxIntentos)
        {
            var keyBloqueo = $"bloqueo_{nombreUsuario.ToLower()}";
            _cache.Set(keyBloqueo, true,
                TimeSpan.FromMinutes(_bloqueoMinutos));
            _cache.Remove(keyIntentos);
        }
    }

    public void LimpiarIntentos(string nombreUsuario)
    {
        _cache.Remove($"intentos_{nombreUsuario.ToLower()}");
        _cache.Remove($"bloqueo_{nombreUsuario.ToLower()}");
    }
}
