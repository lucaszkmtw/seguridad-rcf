using System.Collections.Generic;
using TGP.Seguridad.BussinessLogic.APIRequests;

namespace TGP.Seguridad.Common.APIRequests
{
    public class RequestEditarInfoAcreedor : RequestAPI
    {
        public string Cuit { get; set; }
        public string Email { get; set; }
        public string CodigoAreaTelefono { get; set; }
        public string NumeroTelefono { get; set; }
        public string NumeroInterno { get; set; }
        public string TipoTelefono { get; set; }
    }
}
