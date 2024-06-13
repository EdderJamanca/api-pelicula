using Dapper;
using Microsoft.Data.SqlClient;
using minimalApi.DTOS;
using minimalApi.Identidades;
using System.Data;

namespace minimalApi.Repositorio
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly string? conectionString;
        private readonly HttpContext httpContext;

        public RepositorioPeliculas(IConfiguration configuration, IHttpContextAccessor httpcontextaccesor)
        {
            conectionString = configuration.GetConnectionString("DefaultConnection");
            httpContext = httpcontextaccesor.HttpContext!;
        }

        public async Task<List<Pelicula>> ObtenerPelicula(PaginacionDTO paginacion)
        {
            using (var conexion = new SqlConnection(conectionString))
            {
                var peliculas = await conexion.QueryAsync<Pelicula>("ObtenerPeliculas",
                    new { paginacion.Pagina, paginacion.recordsPorPagina },
                    commandType: CommandType.StoredProcedure);

                var cantidad = await conexion.QueryAsync<int>("cantidadPelicula", commandType: CommandType.StoredProcedure);

                httpContext.Response.Headers.Append("cantidadPelicula", cantidad.ToString());

                return peliculas.ToList();
            }

        }

        public async Task<Pelicula?> ObtenerPeliculaPorId(int idpelicula)
        {
            using (var conexion = new SqlConnection(conectionString))
            {
                using(var multi=await conexion.QueryMultipleAsync("ObtenerPeliculasPorId",new { idpelicula },
                    commandType: CommandType.StoredProcedure))
                {
                    var pelicula=await multi.ReadFirstAsync<Pelicula>();
                    var comentario=await multi.ReadAsync<Comentario>(); 
                    var generos=await multi.ReadAsync<Genero>();
                    var actores =await multi.ReadAsync<ActoresPeliculaDTO>();
                    foreach(var genero in generos)
                    {
                        pelicula.GeneroPeliculas.Add(new GeneroPelicula { idgenero=genero.Id,Genero=genero});
                    }
                    foreach(var actor in actores)
                    {
                        pelicula.ActorPelicula.Add(new ActorPelicula 
                        {
                            idactor=actor.idactores,
                            personajes=actor.personajes,
                            actores=new Actores { nombre=actor.nombre}
                        });
                    }
                    pelicula.comentarios=comentario.ToList();
                    return pelicula;
                }
                //var pelicula = await conexion.QueryFirstOrDefaultAsync<Pelicula>("ObtenerPeliculasPorId",
                //    new {idpelicula},
                //    commandType: CommandType.StoredProcedure);
                //return pelicula;
            }
        }

        public async Task<int> CrearPelicula(Pelicula pelicula)
        {
            using (var conexion = new SqlConnection(conectionString))
            {
                var idpelicula = await conexion.QuerySingleAsync<int>("RegistrarPelicula",
                    new { pelicula.titulo, pelicula.EnCines, pelicula.fechaLanzamiento, pelicula.poster },
                    commandType: CommandType.StoredProcedure);

                pelicula.idpelicula = idpelicula;

                return idpelicula;
            }
        }
        public async Task<bool> Existe(int idpelicula)
        {
            using (var conexion = new SqlConnection(conectionString))
            {
                var existe = await conexion.QuerySingleAsync<bool>(@"
                    if exists(select 1 from peliculas where idpelicula=@idpelicula)
                        select 1;
                    else 
                        select 0;
                ", new { idpelicula });

                return existe;
            }
        }
        public async Task ActualizarPelicula(Pelicula pelicula)
        {
            using (var conxion = new SqlConnection(conectionString))
            {
                await conxion.QueryAsync("ActualizarPelicula", pelicula, commandType: CommandType.StoredProcedure);
            }
        }
        public async Task BorrarPelicula(int idpelicula)
        {
            using (var conexion = new SqlConnection(conectionString))
            {
                await conexion.QueryAsync("EliminarPelicula", new { idpelicula }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AsignarGeneros(int idpelicula,List<int> generosIds)
        {
            var dt = new DataTable();

            dt.Columns.Add("Id",typeof(int));

            foreach(var idgenero in generosIds)
            {
                dt.Rows.Add(idgenero);
            }
            using(var conexion = new SqlConnection(conectionString))
            {
                await conexion.QueryAsync("Pelicula_AsignarGenero",new {idpelicula, generosIds =dt},commandType: CommandType.StoredProcedure);
            }
        }

        public async Task AsignarActores(int idpelicula,List<ActorPelicula> actores)
        {
            for(int i=1; i<=actores.Count; i++)
            {
                actores[i - 1].orden = i;
            }

            var dt= new DataTable();
            dt.Columns.Add("idactor",typeof(int));
            dt.Columns.Add("personaje",typeof(string));
            dt.Columns.Add("orden",typeof(int));

            foreach(var actorPelicula in actores)
            {
                dt.Rows.Add(actorPelicula.idactor, actorPelicula.personajes, actorPelicula.orden);
            }

            using(var conexion = new SqlConnection(conectionString))
            {
                await conexion.QueryAsync("Pelicula_AsignarActores",
                    new {idpelicula,actores=dt},
                    commandType: CommandType.StoredProcedure);  
            }
        }

        public async Task<List<Pelicula>> Filtrar(PeliculaFiltro filtroPeliculaDTO)
        {
            using(var conexion = new SqlConnection(conectionString))
            {
                var peliculas = await conexion.QueryAsync<Pelicula>("Filtro_pelicula", new
                {
                    filtroPeliculaDTO.Pagina,
                    filtroPeliculaDTO.RecordsPorPagina,
                    filtroPeliculaDTO.Titulo,

                    filtroPeliculaDTO.Idgenero,
                    filtroPeliculaDTO.ProximosEstrenos,
                    filtroPeliculaDTO.EnCines,

                    filtroPeliculaDTO.OrdenAscendente,
                    filtroPeliculaDTO.CampoOrdenar
                },commandType:CommandType.StoredProcedure);
            
                var cantidadPeliculas=await conexion.QuerySingleAsync<int>("Pelicula_cantidad",
                    new 
                    {
                        filtroPeliculaDTO.Titulo,
                        filtroPeliculaDTO.Idgenero,
                        filtroPeliculaDTO.ProximosEstrenos,
                        filtroPeliculaDTO.EnCines
                    },commandType: CommandType.StoredProcedure);

                httpContext.Response.Headers.Append("cantidadTotalRegistros",
                    cantidadPeliculas.ToString());

                return peliculas.ToList();
            }
        }
    }
}
