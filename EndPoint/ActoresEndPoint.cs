using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using minimalApi.DTOS;
using minimalApi.Filtros;
using minimalApi.Identidades;
using minimalApi.Repositorio;
using minimalApi.Servicios;
using Microsoft.OpenApi.Models;
using minimalApi.Utilidades;
namespace minimalApi.EndPoint
{
    public static class ActoresEndPoint
    {

        private static readonly string contenedor = "actor";

        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {
            group.MapGet("/", ListaActores).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))).WithTags("actores-get")
                .AgregarParametrosPaginacionAOpenAPI();


            group.MapGet("/{idactores:int}", ActorPorId);
            group.MapGet("/{nombre}", ActorPorNombre);
            group.MapPost("/", crearAutor)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearUpdateActores>>()
                .RequireAuthorization("esadmin")
                .WithOpenApi();

            group.MapPut("/{idactores:int}", actualizarActor)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearUpdateActores>>()
                .RequireAuthorization("esadmin")
                .WithOpenApi();

            group.MapDelete("/{idactores:int}", eliminarAutor)
                .RequireAuthorization("esadmin");
            return group;
        }

        static async Task<Ok<List<ActorDTO>>> ListaActores(IRepositorioActores repositoresActores, 
            IMapper mapper, PaginacionDTO paginacion)
        {
            //var paginacion=new PaginacionDTO { Pagina=pagina,RecordsPorPagina=recordsPorPagina};
            var actores = await repositoresActores.ObtenerActores(paginacion);

            var actoresDTO=mapper.Map<List<ActorDTO>>(actores);

            return TypedResults.Ok(actoresDTO);
        }


        static async Task<Results<Ok<ActorDTO>,NotFound>> ActorPorId(IRepositorioActores repositorioactores, int idactores, IMapper mapper)
        {

            var actor = await repositorioactores.ObtenerActoresPorId(idactores);
            if (actor is null)
            {
                return TypedResults.NotFound();
            }

            var actorDTO= mapper.Map<ActorDTO>(actor);

            return TypedResults.Ok(actorDTO);
        }

        static async Task<Results<Ok<ActorDTO>,NotFound>> ActorPorNombre(IRepositorioActores respositorioactores, string nombre,IMapper mapper)
        {
            var actor = await respositorioactores.ActoresFiltroPorNombre(nombre);

            if(actor is null)
            {
                return TypedResults.NotFound(); 
            }

            var actorDto= mapper.Map<ActorDTO>(actor);

            return TypedResults.Ok(actorDto);
        }
        static async Task<Results<Created<ActorDTO>, ValidationProblem>> crearAutor([FromForm] CrearUpdateActores crearActoresDTO,
            IRepositorioActores repositorioActores,
            IOutputCacheStore outputCacheStore, IMapper mapper,
            IAlamacenadorArchivos almacenadorarchivo)
        {

            var actor = mapper.Map<Actores>(crearActoresDTO);

            if(crearActoresDTO.foto is not null)
            {
                var url = await almacenadorarchivo.Almacenar(contenedor, crearActoresDTO.foto);
                actor.foto = url;
            }


            var id = await repositorioActores.CrearActor(actor);

            await outputCacheStore.EvictByTagAsync("get-actores", default);

            var actoresDTO = mapper.Map<ActorDTO>(actor);

            return TypedResults.Created($"/actores/{id}", actoresDTO);
        }

        static async Task<Results<NoContent,NotFound>> actualizarActor(int idactores,[FromForm] CrearUpdateActores crearActoresDto,
            IRepositorioActores repositorioactores,
            IOutputCacheStore outputCacheStore,IMapper mapper,
            IAlamacenadorArchivos almacenararchivo)
        {
            var actorDB = await repositorioactores.ObtenerActoresPorId(idactores);

            if(actorDB is  null)
            {
                return TypedResults.NotFound();
            }

            var actorParaActualizar = mapper.Map<Actores>(crearActoresDto);
            actorParaActualizar.idactores = idactores;
            actorParaActualizar.foto = actorDB.foto;

            if(crearActoresDto.foto is not null)
            {
                var url=await almacenararchivo.Editar(actorParaActualizar.foto,contenedor,crearActoresDto.foto);
                actorParaActualizar.foto=url;   

            }

            await repositorioactores.ActualizarActores(actorParaActualizar);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent,NotFound>> eliminarAutor(IRepositorioActores repositorio,int idactores,
            IOutputCacheStore outputcahestore,IAlamacenadorArchivos almacenadorarchivo)
        {
            var autorDB = await repositorio.ObtenerActoresPorId(idactores);

            if(autorDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.BorrarActores(idactores);
            await almacenadorarchivo.Borrar(autorDB.foto, contenedor);
            await outputcahestore.EvictByTagAsync("actores-get",default);

            return TypedResults.NoContent();
        }
    }
}
