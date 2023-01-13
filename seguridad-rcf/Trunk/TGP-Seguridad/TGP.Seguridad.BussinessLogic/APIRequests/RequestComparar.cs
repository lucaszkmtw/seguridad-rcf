using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.APIRequests
{
    /// <summary>
    /// Clase que modela el request de un Comparar para traer elementos del destino
    /// </summary>
    public class RequestComparar: RequestAPI
    {

        /// <summary>
        /// Codigo de la estructura funcional a validar
        /// </summary>
        public string CodigoEstructuraFuncional { get; set; }
        /// <summary>
        /// Tipo de elemento a comaprar
        /// </summary>
        public string TipoElemento { get; set; }

    }
}
