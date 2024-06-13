using Dapper;
using Microsoft.Data.SqlClient;
using minimalApi.Identidades;
using System.Data;

namespace minimalApi.Repositorio
{
    public class RepositorioGeneros : IRepositorioGeneros
    {
        // se usa configuracion para acceder a las variables de configuraciones
        // de appsettings.Development

        private readonly string? connectionString;
        public RepositorioGeneros(IConfiguration configuracion)
        {
            connectionString = configuracion.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Genero>> ObtenerTodos()
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                // var generos = await conexion.QueryAsync<Genero>(@"SELECT id,nombre From generos order by nombre");
                var generos = await conexion.QueryAsync<Genero>("Obtener_Genero", commandType: CommandType.StoredProcedure);
                return generos.ToList();
            }
        }

        public async Task<Genero?> ObtenerPorId(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var genero = await conexion.QueryFirstOrDefaultAsync<Genero>(@"select id,nombre from generos where id=@id",new {id});

                return genero;

            }
        }

        public async Task<int> CrearGenero(Genero genero)
        {
            using (var conxion = new SqlConnection(connectionString))
            {
                var id = await conxion.QuerySingleAsync<int>(@"
                            insert into generos (nombre) values(@nombre); select SCOPE_IDENTITY()",genero);

                genero.Id = id;
                return id;       
            }

        }

        public async Task<bool> UnicoGenero(int id,string nombre)
        {
            using(var conxion = new SqlConnection(connectionString))
            {
                var existe = await conxion.QuerySingleAsync<bool>("Generos_ExistePorIdNombre",
                    new {idgenero= id,nombre},commandType:CommandType.StoredProcedure);
                return existe;
            }
        }

        public async Task<bool> Existe(int id)
        {
            using (var conexion = new SqlConnection(connectionString))
            {
                var existe = await conexion.QuerySingleAsync<bool>(@"
                    if exists (select 1 from generos where id=@id)
                        select 1
                    else
                        select 0
                ",new {id});

                return existe;
            }
        }

        public async Task Actualizar(Genero genero)
        {
            using (var conexion=new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync("Actualizar_Genero", genero,commandType:CommandType.StoredProcedure);

            }
        }

        public async Task Borrar(int id)
        {
            using(var conexion=new SqlConnection(connectionString))
            {
                await conexion.ExecuteAsync(@"delete from generos where id=@id",new {id});
            }
        }

        public async Task<List<int>> ExisteGeneros(List<int> generoIds)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id",typeof(int));
            foreach(var idgenero in generoIds)
            {
                dt.Rows.Add(idgenero);
            }

            using (var conexion = new SqlConnection(connectionString))
            {
                var idsGenerosExistentes = await conexion.QueryAsync<int>("Genero_ObtenerVariosPorId",
                    new { generosIds = dt },
                    commandType: CommandType.StoredProcedure);

                return idsGenerosExistentes.ToList();
            }
        }
    }
}
