using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization; 

namespace STGP.Models.DTO
{
    [DataContract]
    public class ResultadoDTO
    {
        /// <summary>
        /// Nro de Codigo de respuesta
        /// </summary>
        [DataMember]
        public int Codigo { get; set; }
        /// <summary>
        /// Descripcion de la respuesta
        /// </summary>
        [DataMember]
        public string Descripcion { get; set; }
    }
}