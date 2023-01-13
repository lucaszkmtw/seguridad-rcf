using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto.APIResponse
{
    public class ResponseAPIAppsMasUsadas : ResponseAPI
    {
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
