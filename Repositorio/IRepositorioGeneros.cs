using minimalApi.Identidades;

namespace minimalApi.Repositorio
{
    public interface IRepositorioGeneros
    {
        Task Actualizar(Genero genero);
        Task Borrar(int id);
        Task<int> CrearGenero(Genero genero);
        Task<bool> Existe(int id);
        Task<List<int>> ExisteGeneros(List<int> generoIds);
        Task<Genero?> ObtenerPorId(int id);
        Task<List<Genero>> ObtenerTodos();

        Task<bool> UnicoGenero(int id, string nombre);
    }
}