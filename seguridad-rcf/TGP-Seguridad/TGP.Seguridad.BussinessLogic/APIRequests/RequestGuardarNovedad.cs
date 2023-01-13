using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.APIRequests
{
    public class RequestGuardarNovedad : RequestAPI
    {
        public long NovedadId { get; set; }
        public string Comentario { get; set; }
    }
}
