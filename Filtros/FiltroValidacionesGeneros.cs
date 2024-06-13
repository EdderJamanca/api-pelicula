using FluentValidation;
using minimalApi.DTOS;
using minimalApi.Repositorio;

namespace minimalApi.Filtros
{
    public class FiltroValidacionesGeneros:IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<CrearUpdateDto>>();

            if(validator is null)
            {
                return await next(context);
            }
            var insumoAValidar= context.Arguments.OfType<CrearUpdateDto>().FirstOrDefault();
            if(insumoAValidar is null)
            {
                return TypedResults.Problem("No pude ser encontrada la entidad a validar");
            }

            var resultadoValidacion = await validator.ValidateAsync(insumoAValidar);

            if (!resultadoValidacion.IsValid)
            {
                return TypedResults.ValidationProblem(resultadoValidacion.ToDictionary());
            }

            return await next(context);
        }
    }
}
