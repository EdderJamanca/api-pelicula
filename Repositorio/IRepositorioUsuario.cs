using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace minimalApi.Repositorio
{
    public interface IRepositorioUsuario
    {
        Task<string> Crear(IdentityUser usuario);
        Task<IdentityUser?> BuscarUsuarioPorEmail(string normalizedEmail);
        Task RemoveClaims(IdentityUser user, IEnumerable<Claim> claims);
        Task AsignarClaims(IdentityUser usuario, IEnumerable<Claim> claims);
        Task<List<Claim>> ObtenerClaims(IdentityUser usuario);
    }
}