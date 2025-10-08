using GestionBiblioteca.Entities;
using GestionBiblioteca.Repository;
using Microsoft.EntityFrameworkCore;

namespace GestionBiblioteca.Services.Prestamo;

public class PrestamoService: IPrestamoService
{
    private readonly IRepositoryFactory _repositoryFactory;

    public PrestamoService(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }
    
    public async Task<List<Entities.Prestamo>> ObtenerTodos()
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Prestamo>().ObtenerPorConsulta()
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.PrestamoEjemplares)
            .Where(p => p.Activo == 1)
            .ToListAsync();
    }
    
    public async Task<Entities.Prestamo?> ObtenerPorId(int id)
    {
        return await _repositoryFactory.ObtenerRepository<Entities.Prestamo>().ObtenerPorConsulta()
            .Include(p => p.IdUsuarioNavigation)
            .Include(p => p.PrestamoEjemplares)
            .ThenInclude(pe => pe.IdEjemplarNavigation)
            .ThenInclude(l => l.IdLibroNavigation)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task Crear(Entities.Prestamo nuevoPrestamo, List<int> ejemplarIds)
    {
        using var transaction = await _repositoryFactory.BeginTransaction();

        try
        {
            // 1️⃣ Create the Prestamo
            await _repositoryFactory.ObtenerRepository<Entities.Prestamo>().Agregar(nuevoPrestamo);

            // 2️⃣ Create PrestamoEjemplares
            foreach (var ejemplarId in ejemplarIds)
            {
                var prestamoEjemplar = new PrestamoEjemplar
                {
                    IdPrestamo = nuevoPrestamo.Id,
                    IdEjemplar = ejemplarId,
                    FechaLimite = DateTime.Now.AddDays(7),
                    Activo = 1
                };

                await _repositoryFactory.ObtenerRepository<PrestamoEjemplar>().Agregar(prestamoEjemplar);
            }

            // 3️⃣ Update each Ejemplar to mark it unavailable
            foreach (var ejemplarId in ejemplarIds)
            {
                var ejemplarRepo = _repositoryFactory.ObtenerRepository<Entities.Ejemplar>();
                var ejemplar = await ejemplarRepo.ObtenerPorId(ejemplarId);
                ejemplar.Disponible = false;
                await ejemplarRepo.Actualizar(ejemplar);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<Entities.Prestamo> PrestarEjemplares(Entities.Prestamo prestamo)
    {
        throw new NotImplementedException();
    }
    
}