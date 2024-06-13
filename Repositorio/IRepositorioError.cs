using minimalApi.Identidades;

namespace minimalApi.Repositorio
{
    public interface IRepositorioError
    {
        Task crear(Error error);
    }
}