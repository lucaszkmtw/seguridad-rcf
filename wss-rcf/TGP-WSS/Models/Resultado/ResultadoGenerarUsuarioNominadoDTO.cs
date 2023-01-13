using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using TGP.WSS.Models.DTO;

namespace TGP.WSS.Models.Resultado
{
    [DataContract]
    public class ResultadoGenerarUsuarioNominadoUsuarioDTO : ResultadoDTO
    {
        [DataMember]
        public string ClaveGenerada { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string NombreUsuario { get; set; }


    }
}