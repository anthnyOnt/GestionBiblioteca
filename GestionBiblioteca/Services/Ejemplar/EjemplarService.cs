using GestionBiblioteca.Repository;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Ejemplar;

public class EjemplarService: IEjemplarService
{
    private readonly IRepositoryFactory _repositoryFactory;
    
    public EjemplarService(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }
    
    public async Task<List<Entities.Ejemplar>> ObtenerDisponibles()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Ejemplar>().ObtenerPorConsulta()
            .Where(e => e.Activo.Equals(1) && e.Disponible == true)
            .Include(e => e.IdLibroNavigation)
            .ToListAsync();
    }

    public async Task<List<Entities.Ejemplar>> ObtenerSeleccionados(List<int> seleccionados)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Ejemplar>().ObtenerPorConsulta()
            .Include(e => e.IdLibroNavigation)
            .Where(e => seleccionados.Contains(e.Id))
            .ToListAsync();
    }
}