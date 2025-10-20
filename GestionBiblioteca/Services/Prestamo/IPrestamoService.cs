using GestionBiblioteca.Entities;

namespace GestionBiblioteca.Services.Prestamo;

public interface IPrestamoService
{
    Task<List<Entities.Prestamo>> ObtenerTodos();
    Task<Entities.Prestamo?> ObtenerPorId(int id);
    Task Crear(int idUsuario, List<PrestamoEjemplar> ejemplares);

}