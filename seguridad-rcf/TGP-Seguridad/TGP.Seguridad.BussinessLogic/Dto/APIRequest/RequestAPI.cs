using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto.APIRequest
{
    /// <summary>
    /// Clase que modela las 2 propiedades basicas de un request a la Api SEG
    /// </summary>
    public class RequestAPI
    {
        /// <summary>
        /// Nombre de usuario a autenticar
        /// </summary>
        public string NombreUsuario { get; set; }
        /// <summary>
        /// Key para autenticar la llamada
        /// </summary>
        public string KeyEncrypt { get; set; }

    }
}
