namespace Sistema.RENACER.Services;

public interface IAuditoriaService
{
    Task RegistrarAsync(string tabla, string operacion, long idRegistro,
        object? valorAnterior, object? valorNuevo, int? idUsuario, string? ip);
}
