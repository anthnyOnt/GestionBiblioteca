namespace GestionBiblioteca.Services.Ejemplar;

public interface IEjemplarService
{
    Task<List<Entities.Ejemplar>> ObtenerDisponibles();
    Task<List<Entities.Ejemplar>> ObtenerSeleccionados(List<int> seleccionados);
    Task<Entities.Ejemplar> Crear(Entities.Ejemplar ejemplar);
    Task<Entities.Ejemplar?> Actualizar(Entities.Ejemplar ejemplar);
    Task<bool> Eliminar(int id);
}