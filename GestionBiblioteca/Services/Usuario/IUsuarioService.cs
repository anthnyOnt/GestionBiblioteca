namespace GestionBiblioteca.Services.Usuario;

public interface IUsuarioService
{
    Task<List<Entities.Usuario>> ObtenerTodos();
    Task<Entities.Usuario?> ObtenerPorId(int id);

    Task<Entities.Usuario> ObtenerPorCi(String ci);
    Task<Entities.Usuario> Crear(Entities.Usuario usuario);
    Task<Entities.Usuario?> Actualizar(Entities.Usuario usuario);
    Task<bool> Eliminar(int id);
}