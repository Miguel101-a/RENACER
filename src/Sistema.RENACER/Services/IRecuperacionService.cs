namespace Sistema.RENACER.Services;

public interface IRecuperacionService
{
    Task<(bool Exito, string? Error, string? Token)> GenerarTokenAsync(string nombreUsuario);
    Task<(bool Exito, string? Error)> CambiarPasswordConTokenAsync(string token,
        string nuevaPassword, string? ip);
}
