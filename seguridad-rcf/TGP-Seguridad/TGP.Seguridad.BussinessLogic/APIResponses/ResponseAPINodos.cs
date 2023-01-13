using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.APIResponses
{
    public class ResponseAPINodos : ResponseAPI
    {
        public List<NodoResponse> Nodos { get; set; }

    }

    public class NodoResponse
    {
        public long IdNodo { get; set; }
        public string DescripcionNodo { get; set; }
        public string CodigoNodo { get; set; }

    }

}
