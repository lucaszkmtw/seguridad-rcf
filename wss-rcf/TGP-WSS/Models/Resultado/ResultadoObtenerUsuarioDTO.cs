using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TGP.WSS.Models.Resultado
{
    /// <summary>
    /// Clase que modela el resultado de obtener un usuario desde el servicio de Seguridad
    /// </summary>
    public class ResultadoObtenerUsuarioDTO :ResultadoDTO
    {
        /// <summary>
        /// Usuario Nominado de las aplicaciones de TGP
        /// </summary>
        [DataMember]
        public DTO.UsuarioNominadoDTO UsuarioNominadoDTO  { get; set; }
    }
}