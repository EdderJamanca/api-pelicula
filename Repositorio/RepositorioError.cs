using Dapper;
using Microsoft.Data.SqlClient;
using minimalApi.Identidades;

namespace minimalApi.Repositorio
{
    public class RepositorioError : IRepositorioError
    {
        private readonly string connectionString;
        public RepositorioError(IConfiguration configuracion)
        {
            connectionString = configuracion.GetConnectionString("DefaultConnection")!;
        }

        public async Task crear(Error error)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("ErroresCrear", new { error.mensajeDeError, error.StackTrace, error.fecha },
                    commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    }
}
