using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.APIResponses
{
    public class ResponseAPIAppsMasUsadas : ResponseAPI
    {
        public ResponseAPIAppsMasUsadas()
        {
            Apps = new List<ResponseAppsMasUsadas>();
        }

        public List<ResponseAppsMasUsadas> Apps { get; set; }

    }

    public class ResponseAppsMasUsadas
    {
        public string CodigoEstructura { get; set; }
        public string DescripcionEstructura { get; set; }
        public string Url { get; set; }
        public int CantApariciones { get; set; }

    }


}
