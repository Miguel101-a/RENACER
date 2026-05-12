using Newtonsoft.Json;
using Sistema.RENACER.Data;
using Sistema.RENACER.Models;

namespace Sistema.RENACER.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly RenacerDbContext _db;

    public AuditoriaService(RenacerDbContext db)
    {
        _db = db;
    }

    public async Task RegistrarAsync(string tabla, string operacion, long idRegistro,
        object? valorAnterior, object? valorNuevo, int? idUsuario, string? ip)
    {
        var registro = new Auditoria
        {
            Tabla = tabla,
            Operacion = operacion,
            IdRegistro = idRegistro,
            ValorAnterior = valorAnterior != null ? JsonConvert.SerializeObject(valorAnterior) : null,
            ValorNuevo = valorNuevo != null ? JsonConvert.SerializeObject(valorNuevo) : null,
            IdUsuario = idUsuario,
            Fecha = DateTime.Now,
            IP = ip
        };
        _db.Auditorias.Add(registro);
        await _db.SaveChangesAsync();
    }
}
