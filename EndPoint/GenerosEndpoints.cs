using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using minimalApi.DTOS;
using minimalApi.Filtros;
using minimalApi.Identidades;
using minimalApi.Repositorio;
using System.Runtime.CompilerServices;

namespace minimalApi.EndPoint
{
    public static class GenerosEndpoints
    {
        
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerGeneros)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)))
                .WithTags("generos-get").RequireAuthorization();

            group.MapGet("/{id:int}", ObtenerGeneroPorId);

            group.MapPost("/", crearGenero).AddEndpointFilter<FiltroValidaciones<CrearUpdateDto>>()
                .RequireAuthorization("esadmin");

            group.MapPut("/{id:int}", updateGenero).AddEndpointFilter<FiltroValidaciones<CrearUpdateDto>>()
                .RequireAuthorization("esadmin");

            group.MapDelete("/{id:int}", EliminarGenero)
                .RequireAuthorization("esadmin");

            return group;
        }

        
        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio,
            IMapper mapper,ILoggerFactory logerFactori)
        {
            // seccion de loger informacion
            var tipo = typeof(GenerosEndpoints);
            var logger=logerFactori.CreateLogger(tipo.FullName!);
            logger.LogTrace("Este es un mensaje de trace");
            logger.LogDebug("Este es un mensaje de debug");
            logger.LogInformation("Este es un mesaje de information");
            logger.LogWarning("Este es un mensaje wairning");
            logger.LogError("Este es un mensaje error");
            logger.LogCritical("Este es un mensaje de critical");
            
            // end seccion

            var generos = await repositorio.ObtenerTodos();

            var generosDto = mapper.Map<List<GeneroDTO>>(generos);
            return TypedResults.Ok(generosDto);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId([AsParameters] ObtenerGeneroPorIdPeticionDTO modelo)
        {
            var genero = await modelo.Repositorio.ObtenerPorId(modelo.Id);

            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            var generosDto=modelo.Mapper.Map<GeneroDTO>(genero);

            return TypedResults.Ok(generosDto);
        }

        static async Task<Results<Created<GeneroDTO>,ValidationProblem>> crearGenero(CrearUpdateDto createGeneroDto, 
                    IRepositorioGeneros repositoriogeneros, 
                    IOutputCacheStore outputCacheStore,
                    IMapper maper)
        {
            var genero = maper.Map<Genero>(createGeneroDto);

            var id = await repositoriogeneros.CrearGenero(genero);
            // esto hace que se limpie la cache
            await outputCacheStore.EvictByTagAsync("generos-get", default);

            var generoDTO = maper.Map<GeneroDTO>(genero);

            return TypedResults.Created($"/generos/{id}", generoDTO);
        }

        static async Task<Results<NotFound, NoContent,ValidationProblem>> updateGenero(int id, CrearUpdateDto updateGeneroDto, 
                IRepositorioGeneros repositoriogeneros, 
                IOutputCacheStore outputCacheStore,
                IMapper maper)
        {
  
            var existe = await repositoriogeneros.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var genero = maper.Map<Genero>(updateGeneroDto);
            genero.Id = id;

            await repositoriogeneros.Actualizar(genero);

            await outputCacheStore.EvictByTagAsync("generos-get", default);

            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> EliminarGenero(int id, IRepositorioGeneros repositoriogeneros, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositoriogeneros.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositoriogeneros.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);

            return TypedResults.NoContent();

        }

    }
}
