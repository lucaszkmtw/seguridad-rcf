using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TGP.WSS.Models.Resultado
{
    /// <summary>
    /// Clase que modela el resultado base de los servicios
    /// </summary>
    [DataContract]
    public class ResultadoDTO
    {
        /// <summary>
        /// Codigo de respuesta del servicio
        /// </summary>
        [DataMember]
        public string Codigo { get; set; }
        /// <summary>
        /// Descripcion con mas detalle de la respuesta
        /// </summary>
        [DataMember]
        public string Descripcion { get; set; }

    }
}