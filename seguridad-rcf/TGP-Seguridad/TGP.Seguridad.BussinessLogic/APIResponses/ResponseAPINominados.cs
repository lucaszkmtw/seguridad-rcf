using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.APIResponses
{
    public class ResponseAPINominados : ResponseAPI
    {
        public List<NominadoResponse> Nominados { get; set; }

    }
    public class NominadoResponse
    {
        public long IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string DescripcionUsuario { get; set; }
        public string MailUsuario { get; set; }
        public virtual byte[] Avatar { get; set; }
        public virtual bool EsContadorDelegado { get; set; }
        public virtual List<string> Nodos { get; set; }
    }

}
