using System.Collections.Generic;
using TGP.Seguridad.BussinessLogic.APIRequests;

namespace TGP.Seguridad.Common.APIRequests
{
    public class RequestGetInfoContactoUsuario : RequestAPI
    {
        public string Cuit { get; set; }
    }
}
