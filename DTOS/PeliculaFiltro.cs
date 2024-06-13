using minimalApi.Utilidades;

namespace minimalApi.DTOS
{
    public class PeliculaFiltro
    {
        public int Pagina { get; set; }
        public int RecordsPorPagina { get; set; }
        public PaginacionDTO PaginacionDTO
        {
            get
            {
                return new PaginacionDTO()
                {
                    Pagina = Pagina,
                    RecordsPorPagina = RecordsPorPagina
                };
                
            }
        }

        public string? Titulo { get; set; } 
        public int Idgenero { get; set; }
        public bool EnCines { get; set; }
        public bool ProximosEstrenos { get; set; }
        public string? CampoOrdenar { get; set; }
        public bool OrdenAscendente { get; set; }
        public static ValueTask<PeliculaFiltro> BindAsync(HttpContext context)
        {
            var pagina = context.ExtraerValorDefecto(nameof(Pagina), 1);
            var recordsPorPagina=context.ExtraerValorDefecto(nameof(RecordsPorPagina), 10);
            var idgenero=context.ExtraerValorDefecto(nameof(Idgenero), 0);

            var titulo=context.ExtraerValorDefecto(nameof(Titulo), String.Empty);
            var enCines= context.ExtraerValorDefecto(nameof(EnCines),false);
            var proximosEstronos=context.ExtraerValorDefecto(nameof(ProximosEstrenos),false);
            var campoOrdenar=context.ExtraerValorDefecto(nameof(CampoOrdenar),String.Empty);
            var ordenAscendente=context.ExtraerValorDefecto(nameof(OrdenAscendente),true);
            var resultado = new PeliculaFiltro
            {
                Pagina = pagina,
                RecordsPorPagina = recordsPorPagina,
                Titulo = titulo,
                Idgenero = idgenero,
                EnCines = enCines,
                ProximosEstrenos = proximosEstronos,
                CampoOrdenar=campoOrdenar,
                OrdenAscendente= ordenAscendente
            };

            return ValueTask.FromResult(resultado);

        }
    }
}
