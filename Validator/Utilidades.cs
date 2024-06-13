namespace minimalApi.Validator
{
    public class Utilidades
    {
        public static string CampoRequeridoMensaje = "El campo {PropertyName} es requerido";
        public static string MaximnLengthMensaje = "El campo {PropertyName} debe tener menos de {MaxLength} caracteres";
        public static string PrimeraLetraMayusculaMensaje = "El campo {PropertyName} debe comenzar con mayúscula";
        public static string EmailMensaje = "El campo {PropertyName} debe ser un email valido";

        public static bool PrimeraLetraEndMayuscula(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return true;
            }
            var primeraLetra = valor[0].ToString();

            return primeraLetra == primeraLetra.ToUpper();
        }

        public static string GreateThanOrEqualToMensaje(DateTime fechaMinima)
        {
            return "El campo {PropertyName} debe ser posterior a" + fechaMinima.ToString("yyyy-MM-dd");
        }
    }
}
