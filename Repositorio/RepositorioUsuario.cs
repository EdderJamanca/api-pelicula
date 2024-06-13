using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using minimalApi.Identidades;
using System.Data;
using System.Security.Claims;


namespace minimalApi.Repositorio
{
    public class RepositorioUsuario : IRepositorioUsuario
    {
        private readonly string connectionString;

        public RepositorioUsuario(IConfiguration configuracion)
        {
            connectionString = configuracion.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IdentityUser?> BuscarUsuarioPorEmail(string normalizedEmail)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var usuario= await conexion.QuerySingleOrDefaultAsync<Usuario>("Usuario_BuscarPorEmail",
                    new { normalizedEmail }, commandType: CommandType.StoredProcedure);
                if(usuario is null)
                {
                    return null;
                }
                var respUsuario = new IdentityUser()
                {
                    Id= usuario.idusuario.ToString(),
                    UserName=usuario.username,
                    NormalizedUserName=usuario.normalizedUserName,
                    Email=usuario.email,
                    NormalizedEmail=usuario.normalizedEmail,
                    EmailConfirmed=usuario.emailConfirmed,
                    PasswordHash=usuario.passwordHash,
                    SecurityStamp=usuario.securityStamp,
                    ConcurrencyStamp=usuario.concurrencyStamp,
                    PhoneNumber=usuario.phoneNumber,
                    PhoneNumberConfirmed=usuario.phoneNumberConfirmed,
                    TwoFactorEnabled=usuario.twoFactorEnabled,
                    LockoutEnd=usuario.lockoutEnd,
                    LockoutEnabled=usuario.lockoutEnabled,
                    AccessFailedCount=usuario.accessFaildCount
                };

                return respUsuario;
            }
        }

        public async Task<string> Crear(IdentityUser usuario)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                usuario.Id = Guid.NewGuid().ToString();
                await conexion.ExecuteAsync("Usuarios_Crear", new
                {
                    idusuario=usuario.Id,
                    email=usuario.Email,
                    normalizedEmail=usuario.NormalizedEmail,
                    userName=usuario.UserName,
                    normalizedUserName=usuario.NormalizedUserName,
                    passwordHash=usuario.PasswordHash
                }, commandType: CommandType.StoredProcedure);

                return usuario.Id;
            }
        }

        public async Task<List<Claim>> ObtenerClaims(IdentityUser usuario)
        {
            using(var conexion = new SqlConnection(connectionString))
            {
                var claims= await conexion.QueryAsync<Claim>("Usuario_ObtenerClaims",new {usuario.Id},commandType:CommandType.StoredProcedure);

                return claims.ToList();
            }

        }

        public async Task AsignarClaims(IdentityUser usuario,IEnumerable<Claim> claims)
        {
            var sql = @"insert into usuariosClaims (idusuario,claimType,claimValue) values(@id,@type,@value)";

            var parametros = claims.Select(x=> new {usuario.Id,x.Type,x.Value});

            using(var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync(sql, parametros);
            }
        }

        public async Task RemoveClaims(IdentityUser user,IEnumerable<Claim> claims)
        {
            var sql = @"delete from usuariosClaims where idusuario=@id and claimType=@type";
            var parametros = claims.Select(x => new {user.Id,x.Type});

            using(var conexion= new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync(sql,parametros);
            }
        }

    }
}
