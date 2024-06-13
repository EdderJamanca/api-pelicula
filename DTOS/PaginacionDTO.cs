using Microsoft.IdentityModel.Tokens;
using minimalApi.Utilidades;

namespace minimalApi.DTOS
{
    public class PaginacionDTO
    {
        private const int paginaValorInicial = 1;
        private const int recordsPorPaginaValorInicial = 10;
        public int Pagina { get; set; } = paginaValorInicial;
        public int recordsPorPagina = recordsPorPaginaValorInicial;
        public readonly int cantidadMaximaRecorPorPagina = 50;

        public int RecordsPorPagina
        {
            get
            {
                return recordsPorPagina;
            }
            set
            {
                recordsPorPagina= (value > cantidadMaximaRecorPorPagina)?cantidadMaximaRecorPorPagina:value;
            }
        }

        public static ValueTask<PaginacionDTO> BindAsync(HttpContext context)
        {
            var pagina = context.ExtraerValorDefecto(nameof(Pagina), paginaValorInicial);
            var recordsPorPagina= context.ExtraerValorDefecto(nameof(RecordsPorPagina),recordsPorPaginaValorInicial);
            //var pagina = context.Request.Query[nameof(Pagina)];
            //var recordsPorPagina = context.Request.Query[nameof(RecordsPorPagina)];

            //var paginaInt=pagina.IsNullOrEmpty()? paginaValorInicial:int.Parse(pagina.ToString());
            //var recordsPorPaginaInt=recordsPorPagina.IsNullOrEmpty() ? recordsPorPaginaValorInicial : int.Parse(recordsPorPagina.ToString());

            var resultado = new PaginacionDTO
            {
                Pagina = pagina,
                RecordsPorPagina = recordsPorPagina
            };

            return ValueTask.FromResult(resultado);
        }
    }
}
