using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.APIRequests
{
    public class RequestEliminar:RequestAPI
    {
        /// <summary>
        /// Tipo de elemento a comaprar
        /// </summary>
        public string TipoElemento { get; set; }
        public int Version { get; set; }
        public long Id { get; set; }
    }
}
