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
    public class RequestCopiarNodoDestino: RequestAPI
    {
        /// <summary>
        /// Codigo de la estructura funcional a validar
        /// </summary>
        public string CodigoEstructuraFuncional { get; set; }
        /// <summary>
        /// Roles que envio a copiarse
        /// </summary>
        public IList<NodoDestino> Nodos{ get; set; }
    }
}
