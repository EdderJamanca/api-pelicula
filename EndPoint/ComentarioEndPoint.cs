using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using minimalApi.DTOS;
using minimalApi.Filtros;
using minimalApi.Identidades;
using minimalApi.Repositorio;
using minimalApi.Servicios;
using minimalApi.Validator;

namespace minimalApi.EndPoint
{
    public static class ComentarioEndPoint
    {
        public static RouteGroupBuilder MapComentario( this RouteGroupBuilder group)
        {
            group.MapGet("/", obtenerComentario).CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60)).Tag("comentario-get").SetVaryByRouteValue(new string[] { "idcomentario" }));
            group.MapGet("/{idcomentario:int}", obtenerComentarioPorId);
            group.MapPost("/", crearComentario).AddEndpointFilter<FiltroValidaciones<CreateUpdateComentario>>()
                .RequireAuthorization();

            group.MapPut("/{idcomentario:int}", ActualizarComentario)
                .AddEndpointFilter<FiltroValidaciones<CreateUpdateComentario>>()
                .RequireAuthorization();

            group.MapDelete("/{idcomentario:int}", BorrarComentario)
                .RequireAuthorization();

            return group;
        }

        static async Task<Ok<List<ComentariosDTO>>> obtenerComentario(IRepositorioComentario repositorio, int idpelicula,
            IMapper mapper,int pagina=1,int recordsPorPagina=10)
        {
            var paginacion = new PaginacionDTO {Pagina= pagina,recordsPorPagina=recordsPorPagina};
            var comentarios = await repositorio.ObtenerComentario(paginacion,idpelicula);

            var comentarioDto=mapper.Map<List<ComentariosDTO>>(comentarios);

            return TypedResults.Ok(comentarioDto);
        }

        static async Task<Results<Ok<ComentariosDTO>,NotFound>> obtenerComentarioPorId(int idcomentario,int idpelicula,
            IRepositorioComentario repositorio,IMapper mapper)
        {
            var comentario = await repositorio.ObtenerComentarioPorId(idcomentario, idpelicula);

            if(comentario is null)
            {
                return TypedResults.NotFound();
            }
            var comentarioDto= mapper.Map<ComentariosDTO>(comentario);

            return TypedResults.Ok(comentarioDto);
        }

        static async Task<Results<Created<ComentariosDTO>,BadRequest<string>>> crearComentario(IRepositorioComentario repositorio,
            IMapper mapper,
            CreateUpdateComentario crearComentario,
            IOutputCacheStore outputcachestore,
            IServicioUsuarios servicioUsuario,
            int idpelicula)
        {
            var comentario=mapper.Map<Comentario>(crearComentario);
            comentario.idpelicula = idpelicula;

            var usuario = await servicioUsuario.ObtenerUsuario();

            if(usuario is null)
            {
                return TypedResults.BadRequest("Usuario no encontrado");
            }

            comentario.idusuario = usuario.Id;

            var id = await repositorio.CrearComentario(comentario);
            comentario.idcomentario = id;
            await outputcachestore.EvictByTagAsync("comentario-get", default);

            var comentarioDto = mapper.Map<ComentariosDTO>(comentario);

            return TypedResults.Created($"comentario/{idpelicula}", comentarioDto);

        }

        static async Task<Results<NoContent,NotFound, ForbidHttpResult>> ActualizarComentario(IRepositorioComentario repositorio,
            int idpelicula,int idcomentario, 
            CreateUpdateComentario updateComentario,
            IOutputCacheStore outputcachestore,
            IServicioUsuarios serviceUsuario) 
        {
            var existe = await repositorio.Existe(idcomentario, idpelicula);

            if(!existe)
            {
                return TypedResults.NotFound();
            }

            var comentarioDB = await repositorio.ObtenerComentarioPorId(idcomentario,idpelicula);

            if(comentarioDB is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await serviceUsuario.ObtenerUsuario();

            if(usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioDB.idusuario != usuario.Id)
            {
                return TypedResults.Forbid();
            }

            comentarioDB.cuerpo=updateComentario.cuerpo;

            await repositorio.actualizar(comentarioDB);
            await outputcachestore.EvictByTagAsync("comenterio-get",default);

            return TypedResults.NoContent();

        }

        static async Task<Results<NotFound,NoContent,ForbidHttpResult>> BorrarComentario(int idpelicula,
            int idcomentario,
            IRepositorioComentario repositorio,
            IServicioUsuarios servicioUsuario,
            IOutputCacheStore outputcachestore)
        {
            var existe=await repositorio.Existe(idcomentario,idpelicula);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var comentarioDB = await repositorio.ObtenerComentarioPorId(idcomentario, idpelicula);

            if (comentarioDB is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuario.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioDB.idusuario != usuario.Id)
            {
                return TypedResults.Forbid();
            }

            await repositorio.Borrar(idcomentario, idpelicula);

            await outputcachestore.EvictByTagAsync("comentario-get", default);

            return TypedResults.NoContent();
        }
    }
}
