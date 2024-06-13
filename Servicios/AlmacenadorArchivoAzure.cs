using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace minimalApi.Servicios
{
    public class AlmacenadorArchivoAzure:IAlamacenadorArchivos
    {
        private  string connectionAzureString;
       public AlmacenadorArchivoAzure(IConfiguration configuration)
        {
            connectionAzureString = configuration.GetConnectionString("AzureStore")!;
        }

        public async Task<string> Almacenar(string? contenedor, IFormFile archivo)
        {
            var cliente = new BlobContainerClient(connectionAzureString, contenedor);
            // esto nos dice si existe esa carpeta ya no se va hacer nada y si ni exite si va a crear uno
            await cliente.CreateIfNotExistsAsync();
            // se importa el Azure.Storage.Blobs.Models
            //cliente.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            cliente.SetAccessPolicy(PublicAccessType.Blob);
            //obtenermos la extension del archivo
            var extensiones = Path.GetExtension(archivo.FileName);
            var nombreArchivo= $"{Guid.NewGuid()}{extensiones}";
            var blob = cliente.GetBlobClient(nombreArchivo);
            // asignado en la cabecera el tipo de archivo
            var blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType =archivo.ContentType;
            await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHeaders);

            return blob.Uri.ToString();


        }

        public async Task Borrar(string? ruta,string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return;
            }

            var cliente = new BlobContainerClient(connectionAzureString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            var nombreArchivo=Path.GetFileName(ruta);
            var blob = cliente.GetBlobClient(nombreArchivo);
            await blob.DeleteIfExistsAsync();
        }
    }
}
