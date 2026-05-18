using Sistema.RENACER.Models;

namespace Sistema.RENACER.Services;

public interface ICatalogoService
{
    Task<IEnumerable<TipoCita>> ObtenerTiposCitaAsync();
    Task<IEnumerable<Servicio>> ObtenerServiciosActivosAsync();
    Task<IEnumerable<ProfesionalExterno>> ObtenerProfesionalesActivosAsync();
}
