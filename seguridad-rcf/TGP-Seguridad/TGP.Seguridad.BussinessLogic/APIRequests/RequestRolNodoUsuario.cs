using System.Collections.Generic;
using TGP.Seguridad.BussinessLogic.APIRequests;

namespace TGP.Seguridad.Common.APIRequests
{
    public class RequestRolNodoUsuario : RequestAPI
    {
        public long? IdRol { get; set; }
        public List<long> IdNodos { get; set; }
        public long IdUsuario { get; set; }
    }
}
