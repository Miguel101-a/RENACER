using Sistema.RENACER.Models;

namespace Sistema.RENACER.Services;

public record LoginResultado(bool Exito, string? MensajeError, Usuario? Usuario);

public interface IAuthService
{
    Task<LoginResultado> AutenticarAsync(string nombreUsuario, string password, string? ip);
    bool EstaBloqueado(string nombreUsuario);
    void RegistrarIntentoFallido(string nombreUsuario);
    void LimpiarIntentos(string nombreUsuario);
}
