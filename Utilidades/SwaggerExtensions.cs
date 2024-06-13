using Microsoft.AspNetCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;


namespace minimalApi.Utilidades
{
    public static class SwaggerExtensions
    {
        public static TBuilder AgregarParametrosPaginacionAOpenAPI<TBuilder>(this TBuilder builder)
            where TBuilder : IEndpointConventionBuilder 
        {
            return builder.WithOpenApi(opciones =>
            {
                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "pagina",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default=new OpenApiInteger(1)
                    }
                });
                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "recordsPorPagina",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(10)
                    }
                });
                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name="titulo",
                    In = ParameterLocation.Query,
                    Schema= new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "enCines",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "boolean"
                    }
                });
                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "proximosEstrenos",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "boolean"
                    }
                });
                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "Idgeneros",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "Integer"
                    }
                });
                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name= "CampoOrdenar",
                    In= ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type= "string",
                        Enum= new List<IOpenApiAny>
                        {
                            new OpenApiString("titulo"),
                            new OpenApiString("fechaLanzamiento")
                        }
                    }
                });
                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "OrdenAscendente",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "boolean",
                        Default=new OpenApiBoolean(true)
                    }
                });
                return opciones;
            });
        }
    }
}
