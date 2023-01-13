using System.Collections.Generic;
using TGP.Seguridad.BussinessLogic.APIRequests;

namespace TGP.Seguridad.Common.APIRequests
{
    public class RequestNominados : RequestAPI
    {
        public List<string> CodigosRoles { get; set; }
    }
}
