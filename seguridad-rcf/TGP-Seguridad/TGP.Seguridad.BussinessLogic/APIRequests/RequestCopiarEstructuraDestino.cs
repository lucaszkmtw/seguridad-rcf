using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Mapping;

namespace TGP.Seguridad.BussinessLogic.APIRequests
{
    /// <summary>
    /// Clase que modela un request para copiar roles a destino
    /// </summary>
    public class RequestCopiarEstructuraDestino: RequestAPI
    {
        /// <summary>
        /// Roles que envio a copiarse
        /// </summary>
        public IList<EstructuraDestino> Estructuras { get; set; }
    }
}
