
using TGP.WSS;

namespace TGP.WSS.API.Models.DTO.Comparador
{
    public class ResponseGetEntidadesDTO
    {
        public static string CodigoResponseSuccess = "200";
        public static string MensajeRepsonseSuccess = "Success";
        public static string CodigoUsuarioIncorrecto = "100";
        public static string MensajeUsuarioIncorrecto = "El nombre de usuario es incorrecto.";
        public static string CodigoKeyIncorrecta = "101";
        public static string MensajeKeyIncorrecta = "La key de validación es incorrecta.";
        public static string CodigoOtros = "999";

        /// <summary>
        /// Codigo de la respuesta
        /// </summary>
        public string Codigo { get; set; }
        /// <summary>
        /// Mensaje de la respues
        /// </summary>
        public string Mensaje { get; set; }


    }
}