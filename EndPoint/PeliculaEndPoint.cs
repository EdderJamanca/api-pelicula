using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using minimalApi.DTOS;
using minimalApi.Filtros;
using minimalApi.Identidades;
using minimalApi.Repositorio;
using minimalApi.Servicios;
using minimalApi.Utilidades;

namespace minimalApi.EndPoint
{
    public static class PeliculaEndPoint
    {
        private static readonly string contenedor = "pelicula";

        public static RouteGroupBuilder MapPelicula(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerPelicula).CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60))).WithTags("pelicula-get")
                .AgregarParametrosPaginacionAOpenAPI();

            group.MapGet("/{idpelicula:int}", ObtenerPeliculaPorId);

            group.MapPost("/", crearPelicula)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CreateUpdatePeliculaDTO>>()
                .RequireAuthorization("esadmin")
                .WithOpenApi();

            group.MapPut("/{idpelicula:int}", ActualizarPelicula)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CreateUpdatePeliculaDTO>>()
                .RequireAuthorization("esadmin")
                .WithOpenApi(opciones =>
                {
                    opciones.Summary = "Actualizar un género";
                    opciones.Description = "Con este endpoint podemos actualizar un género";
                    opciones.Parameters[0].Description = "El id del género a actualizar";
                    opciones.RequestBody.Description = "El género que se desea actualizar";
                    return opciones;
                });


            group.MapDelete("/{idpelicula:int}",BorrarPelicula);
            group.MapPost("/{idpelicula:int}/asignarGenero", AsignarGenero)
                .RequireAuthorization("esadmin");
            group.MapPost("/{idpelicula:int}/asignaractores", AsignarAcotres)
                .RequireAuthorization("esadmin");

            group.MapGet("/filtrar", FiltrarPeliculas).AgregarParametrosPaginacionAOpenAPI();
            return group;
        }
        //int paginacion=1,int recordsPorPagina=10
        static async Task<Ok<List<PeliculaDTO>>> ObtenerPelicula(IRepositorioPeliculas repositoriopelicula,
            IMapper mapper,PaginacionDTO paginacion)
        {
           // var paginacion = new PaginacionDTO { Pagina = paginacion,RecordsPorPagina=recordsPorPagina};

            var peliculas = await repositoriopelicula.ObtenerPelicula(paginacion);

            var peliculaDto=mapper.Map<List<PeliculaDTO>>(peliculas);

            return TypedResults.Ok(peliculaDto);
        }
    
        static async Task<Results<Ok<PeliculaDTO>,NotFound>> ObtenerPeliculaPorId(IRepositorioPeliculas repositorio,
            int idpelicula,
            IMapper mapper)
        {
            var pelicula = await repositorio.ObtenerPeliculaPorId(idpelicula);

            if(pelicula is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaDto=mapper.Map<PeliculaDTO>(pelicula);

            return TypedResults.Ok(peliculaDto);

        }

        static async Task<Created<PeliculaDTO>> crearPelicula([FromForm] CreateUpdatePeliculaDTO crearDto,
            IRepositorioPeliculas repositorio,IMapper mapper,
            IAlamacenadorArchivos almacenararchivo,
            IOutputCacheStore outputcachestore)
        {
            var pelicula = mapper.Map<Pelicula>(crearDto);

            if(crearDto.poster is not null)
            {
                var url=await almacenararchivo.Almacenar(contenedor, crearDto.poster);

                pelicula.poster=url;
            }

            var id=await repositorio.CrearPelicula(pelicula);

            await outputcachestore.EvictByTagAsync("pelicula-get", default);

            var peliculaDto = mapper.Map<PeliculaDTO>(pelicula);

            return TypedResults.Created($"/pelicula/{id}", peliculaDto);
        }

        static async Task<Results<NoContent,NotFound>> ActualizarPelicula([FromForm] CreateUpdatePeliculaDTO updateDto,
            int idpelicula,
            IRepositorioPeliculas repositorio,
            IMapper mapper,
            IAlamacenadorArchivos almacenadorArchivo,
            IOutputCacheStore outputcachestore)
        {
            var peliculaDB = await repositorio.ObtenerPeliculaPorId(idpelicula);

            if (peliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaActualizar = mapper.Map<Pelicula>(updateDto);
            peliculaActualizar.idpelicula = idpelicula;
            
            if(updateDto.poster is not null)
            {
                var url= await almacenadorArchivo.Editar(peliculaActualizar.poster,contenedor, updateDto.poster);
                peliculaActualizar.poster = url;
            }

            await repositorio.ActualizarPelicula(peliculaActualizar);

            await outputcachestore.EvictByTagAsync("pelicula-get",default);

            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound,NoContent>> BorrarPelicula(int idpelicula,
            IRepositorioPeliculas repositorio,
            IAlamacenadorArchivos almacenadorArchivo,
            IOutputCacheStore outputCacheStore)
        {
            var peliculaDB= await repositorio.ObtenerPeliculaPorId(idpelicula);

            if(peliculaDB is null)
            {
                return TypedResults.NotFound();
            }
            await repositorio.BorrarPelicula(idpelicula);
            await almacenadorArchivo.Borrar(peliculaDB.poster,contenedor);
            await outputCacheStore.EvictByTagAsync("pelicula-get", default);

            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent,NotFound,BadRequest<string>>> AsignarGenero(int idpelicula,
            List<int> generosIds,
            IRepositorioPeliculas repositorioPelicula,IRepositorioGeneros repositorioGenero)
        {
            if(!await repositorioPelicula.Existe(idpelicula))
            {
                return TypedResults.NotFound();
            }

            var generosExistentes= new List<int>();

            if(generosIds.Count != 0)
            {
                generosExistentes= await repositorioGenero.ExisteGeneros(generosIds);
            }

            if(generosExistentes.Count !=  generosIds.Count())
            {
                var generosNoExistentes=generosIds.Except(generosExistentes);

                return TypedResults.BadRequest($"Los géneros de id {string.Join(",", generosNoExistentes)} no existen.");
            }

            await repositorioPelicula.AsignarGeneros(idpelicula, generosIds);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AsignarAcotres(int idpelicula,
            List<AsignarActorPeliculaDTO> actoresDTO, IRepositorioPeliculas repositoriopelicula,
            IRepositorioActores repositorioactores, IMapper mapper)
        {
            if (!await repositoriopelicula.Existe(idpelicula))
            {
                return TypedResults.NotFound();
            }
            var actoresExistentes = new List<int>();
            var actoresIds = actoresDTO.Select(a => a.idactor).ToList();

            if (actoresDTO.Count != 0)
            {
                actoresExistentes = await repositorioactores.ExistenActores(actoresIds);
            }

            if (actoresExistentes.Count != actoresDTO.Count)
            {
                var actoresNoExistentes = actoresIds.Except(actoresExistentes);
                return TypedResults.BadRequest($"Los actores de id {string.Join(",", actoresNoExistentes)} no existen");
            }

            var actores = mapper.Map<List<ActorPelicula>>(actoresDTO);
            await repositoriopelicula.AsignarActores(idpelicula, actores);
            return TypedResults.NoContent();
        }

        static async Task<Ok<List<PeliculaDTO>>> FiltrarPeliculas(PeliculaFiltro peliculasFiltrarDTO,
            IRepositorioPeliculas repositorio,IMapper mapper)
        {
            var peliculas=await repositorio.Filtrar(peliculasFiltrarDTO);
            var peliculasDTO=mapper.Map<List<PeliculaDTO>>(peliculas);

            return TypedResults.Ok(peliculasDTO);
        }
    }
}
