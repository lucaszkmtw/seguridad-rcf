using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TGP.WSS.Models.DTO
{
    public class ResultadoObtenerNodosFuncionalesYUsuariosDTO : Resultado.ResultadoDTO
    {
        /// <summary>
        /// Listado de nodos funcionales
        /// </summary>
        [DataMember]
        public List<NodoFuncionalDTO> NodosFuncionales { get; set; }
    }
}