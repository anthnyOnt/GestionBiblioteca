using GestionBiblioteca.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace GestionBiblioteca.Repository;

public class RepositoryFactory: IRepositoryFactory
{
    private readonly MyDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public RepositoryFactory(MyDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public IRepository<T> ObtenerRepository<T>() where T : class
    {
        return new Repository<T>(_context);
    }

    public async Task<IDbContextTransaction> BeginTransaction()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}