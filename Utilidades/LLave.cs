using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using minimalApi.DTOS;

namespace minimalApi.Utilidades
{
    public static class LLave
    {
        public const string IssuerPropio = "nuestra-app";
        public const string SeccionLlaves = "Authentication:Schemes:Bearer:SigningKeys";
        public const string SeccionLlaves_Emisor = "Issuer";
        public const string SeccionLlaves_Valor = "Value";

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuracion)
            =>ObtenerLlave(configuracion, IssuerPropio);
        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration, string issuer)
        {
                var segningkey= configuration.GetSection(SeccionLlaves).GetChildren().SingleOrDefault(llave => llave[SeccionLlaves_Emisor] == issuer);

            if(segningkey is not null && segningkey[SeccionLlaves_Valor] is string valorLlave)
            {
                yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
            }
        }


        public static IEnumerable<SecurityKey> ObtenerTodasLasLlaves(IConfiguration configuration)
        {
            var segningkeys = configuration.GetSection(SeccionLlaves)
            .GetChildren();
            //.SingleOrDefault(llave => llave[SeccionLlaves] == issuer);
            foreach (var segningkey in segningkeys)
            {
                if (segningkey[SeccionLlaves_Valor] is string valorLlave)
                {
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
                }
            }

        }
    }
}
