using System.Collections.Generic;
using TGP.Seguridad.BussinessLogic.APIRequests;

namespace TGP.Conciliacion.Common.APIRequests
{
    public class RequestRoles : RequestAPI
    {
        public string CodEstructuraFuncional { get; set; }
        public List<string> CodRoles{ get; set; }
    }
}
