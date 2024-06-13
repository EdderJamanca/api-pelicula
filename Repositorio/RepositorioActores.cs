using Dapper;
using Microsoft.Data.SqlClient;
using minimalApi.DTOS;
using minimalApi.Identidades;
using System.Data;

namespace minimalApi.Repositorio
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly string connectionString;
        private readonly HttpContext httpContext;

        public RepositorioActores(IConfiguration configuracion,
            IHttpContextAccessor httpContextAccessor)
        {
            connectionString = configuracion.GetConnectionString("DefaultConnection")!;

            httpContext=httpContextAccessor.HttpContext!;

        }
        public async Task<List<Actores>> ObtenerActores(PaginacionDTO paginacionDto)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var actor = await conexion.QueryAsync<Actores>("ObtenerActores",
                    new {paginacionDto.Pagina,paginacionDto.recordsPorPagina}, commandType: CommandType.StoredProcedure);

                var cantidad=await conexion.QueryAsync<int>("cantidad_actores",commandType:CommandType.StoredProcedure);

                httpContext.Response.Headers.Append("catidadTotalRegistro",cantidad.ToString());


                return actor.ToList();
            }
        }

        public async Task<Actores?> ActoresFiltroPorNombre(string nombre)
        {
            using(var conexion = new SqlConnection(this.connectionString))
            {
                var actor= await conexion.QueryFirstOrDefaultAsync<Actores>("ObtenerFiltroPorNombre", new { nombre },commandType:CommandType.StoredProcedure);

                return actor;
            }
        }
        public async Task<Actores?> ObtenerActoresPorId(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var actor = await conexion.QueryFirstOrDefaultAsync<Actores>("ObtenerActorPorId", new { idactores = id }, commandType: CommandType.StoredProcedure);

                return actor;
            }
        }
        public async Task<int> CrearActor(Actores actores)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
               
                var id = await conexion.QuerySingleAsync<int>("crearActor", new { actores.nombre, actores.fechaNacimiento, actores.foto },
                    commandType: CommandType.StoredProcedure);
                actores.idactores = id;
                return id;
            }
        }

        public async Task<bool> ExisteActor(int idactores)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var exite = await conexion.QuerySingleAsync<bool>(@"
                    if exists (select 1 from actores where id=@id)
                       select 1
                    else
                        select 0
                    end
                ", new { idactores });
                return exite;
            }
        }
        public async Task ActualizarActores(Actores actores)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.QueryAsync("actualizarActor", actores, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task BorrarActores(int idactores)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.QueryAsync("EliminarActor", new { idactores }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<int>> ExistenActores(List<int> idsactore)
        {
            var dt=new DataTable();
            dt.Columns.Add("Id",typeof(int));

            foreach(var id in idsactore)
            {
                dt.Rows.Add(id);
            }
            using(var conexion = new SqlConnection(connectionString))
            {
                var idsGenerosExistentes=await conexion.QueryAsync<int>("Actores_obtenerVariosPorId",
                    new { actoresIds = dt },commandType: CommandType.StoredProcedure);
                return idsGenerosExistentes.ToList();
            }
        }

    }
}
