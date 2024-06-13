using Dapper;
using Microsoft.Data.SqlClient;
using minimalApi.DTOS;
using minimalApi.Identidades;
using System.Data;

namespace minimalApi.Repositorio
{
    public class RepositorioComentario : IRepositorioComentario
    {
        private readonly string conexionString;
        private readonly HttpContext httpcontext;
        public RepositorioComentario(IConfiguration configuration, IHttpContextAccessor httpcontextaccessor)
        {
            conexionString = configuration.GetConnectionString("DefaultConnection")!;
            httpcontext = httpcontextaccessor.HttpContext!;
        }

        public async Task<List<Comentario>> ObtenerComentario(PaginacionDTO paginacion,int idpelicula)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                var comentarios = await conexion.QueryAsync<Comentario>("ObtenerComentario",
                    new { idpelicula, paginacion.Pagina, paginacion.RecordsPorPagina},
                    commandType: CommandType.StoredProcedure);
                var cantidad = await conexion.QueryAsync<int>("CantidadComentario",new { idpelicula }, commandType: CommandType.StoredProcedure);

                httpcontext.Response.Headers.Append("cantidadComentario", cantidad.ToString());

                return comentarios.ToList();
            }
        }

        public async Task<Comentario?> ObtenerComentarioPorId(int idcomentario, int idpelicula)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                var comentario = await conexion.QueryFirstOrDefaultAsync<Comentario>("ObtenerComentarioPorId",
                    new { idcomentario,idpelicula },
                    commandType: CommandType.StoredProcedure);

                return comentario;

            }
        }

        public async Task<int> CrearComentario(Comentario comentario)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                var idcomentario = await conexion.QuerySingleAsync<int>("CrearComentario",
                    new { cuerpo=comentario.cuerpo, idpelicula=comentario.idpelicula, idusuario=comentario.idusuario },
                    commandType: CommandType.StoredProcedure);

                comentario.idcomentario = idcomentario;
                return idcomentario;
            }
        }

        public async Task<bool> Existe(int idcomentario, int idpelicula)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                var existeComen = await conexion.QuerySingleAsync<bool>(@"
                        if exists( select 1 from comentario where idcomentario =@idcomentario and idpelicula=@idpelicula)
                            select 1
                         else
                            select 0
                    ", new { idcomentario, idpelicula });

                return existeComen;
            }
        }

        public async Task actualizar(Comentario comentario)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                await conexion.QueryAsync("ActualizarComentario", comentario, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Borrar(int idcomentario,int idpelicula)
        {
            using (var conexion = new SqlConnection(conexionString))
            {
                await conexion.QueryAsync("BorrarComentario", new { idcomentario, idpelicula }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
