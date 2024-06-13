using Microsoft.AspNetCore.Identity;

namespace minimalApi.Servicios
{
    public interface IServicioUsuarios
    {
        Task<IdentityUser?> ObtenerUsuario();
    }
}