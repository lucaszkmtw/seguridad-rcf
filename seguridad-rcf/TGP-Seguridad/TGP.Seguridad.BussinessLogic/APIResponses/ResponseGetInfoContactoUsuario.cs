

using System.Collections.Generic;
using TGP.Seguridad.BussinessLogic.Dto;

namespace TGP.Seguridad.BussinessLogic.APIResponses
{
    public class ResponseGetInfoContactoUsuario : ResponseAPI
    {
        public string Email{ get; set; }
        public string CodigoAreaTelefono { get; set; }
        public string NumeroTelefono { get; set; }
        public string NumeroInterno { get; set; }
        public string TipoTelefono { get; set; }
    }
}
