using Microsoft.EntityFrameworkCore.Storage;

namespace GestionBiblioteca.Repository;

public interface IRepositoryFactory
{
    IRepository<T> ObtenerRepository<T>() where T : class;
    Task<IDbContextTransaction> BeginTransaction();

}