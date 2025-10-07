namespace GestionBiblioteca.Services.Categoria;

public interface ICategoriaService
{
    Task<List<Entities.Categoria>> ObtenerTodos();
    Task<Entities.Categoria?> ObtenerPorId(int id);
    Task<Entities.Categoria> Crear(Entities.Categoria categoria);
    Task<Entities.Categoria?> Actualizar(Entities.Categoria categoria);
    Task<bool> Eliminar(int id);
}