using Sistema.RENACER.Models;
using Sistema.RENACER.Repositories;

namespace Sistema.RENACER.Services;

public class CatalogoService : ICatalogoService
{
    private readonly ICatalogoRepository _repo;

    public CatalogoService(ICatalogoRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<TipoCita>> ObtenerTiposCitaAsync()
        => _repo.ObtenerTiposCitaAsync();

    public Task<IEnumerable<Servicio>> ObtenerServiciosActivosAsync()
        => _repo.ObtenerServiciosActivosAsync();

    public Task<IEnumerable<ProfesionalExterno>> ObtenerProfesionalesActivosAsync()
        => _repo.ObtenerProfesionalesActivosAsync();
}
