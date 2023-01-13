using TGP.Seguridad.BussinessLogic.APIRequests;

namespace TGP.Conciliacion.Common.APIRequests
{
    public class RequestNodos : RequestAPI
    {
        public string CodEstructuraFuncional { get; set; }
        public long? IdNominado { get; set; }

    }
}
