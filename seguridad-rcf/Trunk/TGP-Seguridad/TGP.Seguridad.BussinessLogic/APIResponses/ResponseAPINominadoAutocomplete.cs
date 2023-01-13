using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.APIResponses
{
    public class ResponseAPINominadoAutocomplete : ResponseAPI
    {
        public List<NominadoAutocompleteResponse> Nominados { get; set; }

    }

    public class NominadoAutocompleteResponse
    {
        public long IdUsuario { get; set; }
        public string DescripcionUsuario { get; set; }
        //public long DniUsuario { get; set; }
    }

}
