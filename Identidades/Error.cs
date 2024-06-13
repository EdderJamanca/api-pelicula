using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace minimalApi.Identidades
{
    public class Error
    {
        public Guid iderror {  get; set; }
        public string mensajeDeError { get; set; } = null!;
        public string? StackTrace {  get; set; }
        public DateTime fecha { get; set; }
    }
}
