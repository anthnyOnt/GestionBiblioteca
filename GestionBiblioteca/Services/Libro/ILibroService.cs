namespace GestionBiblioteca.Services.Libro;

public interface ILibroService
{
    Task<List<Entities.Libro>> ObtenerTodos();
    Task<Entities.Libro?> ObtenerPorId(int id);
    Task<List<Entities.Libro>> ObtenerEjemplaresPorTitulo(String titulo);
    Task<Entities.Libro> Crear(Entities.Libro libro);
    Task<Entities.Libro?> Actualizar(Entities.Libro libro);
    Task<bool> Eliminar(int id);
}