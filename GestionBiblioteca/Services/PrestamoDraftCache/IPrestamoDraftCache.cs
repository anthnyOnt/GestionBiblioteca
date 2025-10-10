namespace GestionBiblioteca.Services.PrestamoDraftCache
{
    public interface IPrestamoDraftCache
    {
        Task AddAsync(int usuarioId, int ejemplarId);
        Task RemoveAsync(int usuarioId, int ejemplarId);
        Task<IReadOnlyList<int>> GetAllAsync(int usuarioId);
        Task ClearAsync(int usuarioId);
    }
}