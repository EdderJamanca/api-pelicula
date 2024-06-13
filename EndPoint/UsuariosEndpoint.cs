using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using minimalApi.DTOS;
using minimalApi.Filtros;
using minimalApi.Servicios;
using minimalApi.Utilidades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace minimalApi.EndPoint
{
    public static class UsuariosEndpoint
    {
        public static RouteGroupBuilder MapUsuario(this RouteGroupBuilder group)
        {
            group.MapPost("/login", Login)
                .AddEndpointFilter<FiltroValidaciones<CredencialesUsuarioDTO>>();

            group.MapPost("/registrar", Registarar)
                .AddEndpointFilter<FiltroValidaciones<CredencialesUsuarioDTO>>();

            group.MapPost("/haceradmin", HacerAdmin).AddEndpointFilter<FiltroValidaciones<EditarClaimDTO>>()
            .RequireAuthorization("esadmin");

            group.MapPost("/removeradmin", RemoverAdmin).AddEndpointFilter<FiltroValidaciones<EditarClaimDTO>>()
                .RequireAuthorization("esadmin");

            group.MapGet("/renovacionToken", RenovarTokem).RequireAuthorization();
            return group;
        }

        static async Task<Results<Ok<RespuestaAutenticaionDTO>, BadRequest<IEnumerable<IdentityError>>>> Registarar(
            CredencialesUsuarioDTO credencialesUsuarioDTO,
            [FromServices] UserManager<IdentityUser> userManager,
            IConfiguration configuracion)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuarioDTO.Email,
                Email = credencialesUsuarioDTO.Email
            };


            var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password);

            if (resultado.Succeeded)
            {
                var credencialesRespuesta = await ConstruirToken(credencialesUsuarioDTO, configuracion, userManager);
                return TypedResults.Ok(credencialesRespuesta);
            }
            else
            {
                return TypedResults.BadRequest(resultado.Errors);
            }
        }
        public static async Task<Results<Ok<RespuestaAutenticaionDTO>, BadRequest<string>>> Login(
        CredencialesUsuarioDTO credencialesUsuarioDto,
        [FromServices] SignInManager<IdentityUser> signInManager,
        [FromServices] UserManager<IdentityUser> userManeger, IConfiguration configuracion)
        {
            var usuario = await userManeger.FindByEmailAsync(credencialesUsuarioDto.Email);
            if (usuario is null)
            {
                return TypedResults.BadRequest("Login incorrecto");
            }

            var resultado = await signInManager.CheckPasswordSignInAsync(usuario, credencialesUsuarioDto.Password, lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                var respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDto, configuracion, userManeger);

                return TypedResults.Ok(respuestaAutenticacion);
            }
            else
            {
                return TypedResults.BadRequest("Login incorrecto");
            }
        }
        private static async Task<RespuestaAutenticaionDTO> ConstruirToken(CredencialesUsuarioDTO credencialesUsuarioDto,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager)
        {
            var claims = new List<Claim>
            {
                new Claim("email",credencialesUsuarioDto.Email),
                new Claim("lo que yo quiero","culaquier otro valor")
            };

            var usuario = await userManager.FindByNameAsync(credencialesUsuarioDto.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario!);

            claims.AddRange(claimsDB);

            var llave = LLave.ObtenerLlave(configuration);
            var creds = new SigningCredentials(llave.First(), SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var tokenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion,
                signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);

            return new RespuestaAutenticaionDTO
            {
                Token = token,
                Expiracion = expiracion,
            };
        }

        static async Task<Results<NoContent, NotFound>> HacerAdmin(EditarClaimDTO editarClaimDto,
            [FromServices] UserManager<IdentityUser> userManager)
        {
            var usuario = await userManager.FindByEmailAsync(editarClaimDto.Email);

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }
            await userManager.AddClaimAsync(usuario, new Claim("esadmin", "true"));

            return TypedResults.NoContent();

        }

        static async Task<Results<NoContent, NotFound>> RemoverAdmin(EditarClaimDTO editarClaimDTO,
            [FromServices] UserManager<IdentityUser> userManager)
        {
            var usuario = await userManager.FindByEmailAsync(editarClaimDTO.Email);

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }
            var claims = new List<Claim>()
             {
                 new Claim("esadmin","true")
             };
            await userManager.RemoveClaimsAsync(usuario, claims);

            return TypedResults.NoContent();
        }

        public async static Task<Results<Ok<RespuestaAutenticaionDTO>,NotFound>> RenovarTokem(
            IServicioUsuarios servicioUsuario,
            IConfiguration configuracion,
            [FromServices] UserManager<IdentityUser> userManager
         )
        {
            var usuario = await servicioUsuario.ObtenerUsuario();
            if(usuario is null)
            {
                return TypedResults.NotFound();
            }

            var credencialesUsuarioDTO = new CredencialesUsuarioDTO { Email = usuario.Email! };

            var respuestaAutenticacionDTO =await ConstruirToken(credencialesUsuarioDTO,configuracion,userManager);


            return TypedResults.Ok(respuestaAutenticacionDTO);
        }
    }
}
