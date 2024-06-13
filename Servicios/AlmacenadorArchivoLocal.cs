

namespace minimalApi.Servicios
{
    public class AlmacenadorArchivoLocal : IAlamacenadorArchivos
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        //IWebHostEnvironment -> nos permite encontrar la ubicación de la carpeta wwwroot donde se guarda los archivos estaticos
        // IHttpContextAccessor -> nos permite tener acceso al la carpeta wwwroot
        public AlmacenadorArchivoLocal(IWebHostEnvironment env,IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> Almacenar(string? contenedor, IFormFile archivo)
        {
            var extension=Path.GetExtension(archivo.FileName);

            var nombreArchivo=$"{Guid.NewGuid()}{extension}";

            string folder=Path.Combine(env.WebRootPath,contenedor);

            if(!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string ruta=Path.Combine(folder,nombreArchivo);

            using (var ms=new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                //arreglo de bits
                var contenido=ms.ToArray();
                await File.WriteAllBytesAsync(ruta, contenido);
            }
            var url = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var urlArchivo=Path.Combine(url,contenedor,nombreArchivo).Replace("\\","/");
            return urlArchivo;

        }

        public Task Borrar(string? ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return Task.CompletedTask;
            }

            var nombreArchivo = Path.Combine(ruta);
            var directorioArchivo=Path.Combine(env.WebRootPath,contenedor,nombreArchivo);
            if (File.Exists(directorioArchivo))
            {
                File.Delete(directorioArchivo);
            }

            return Task.CompletedTask;
        }
    }
}
