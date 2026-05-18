using Sistema.RENACER.Models;

namespace Sistema.RENACER.Repositories;

public interface ICatalogoRepository
{
    Task<IEnumerable<TipoCita>> ObtenerTiposCitaAsync();
    Task<IEnumerable<Servicio>> ObtenerServiciosActivosAsync();
    Task<IEnumerable<ProfesionalExterno>> ObtenerProfesionalesActivosAsync();
}
