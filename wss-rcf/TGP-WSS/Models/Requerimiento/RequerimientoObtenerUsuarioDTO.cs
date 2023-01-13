using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TGP.WSS.Models.Requerimiento
{
    [DataContract]
    public class RequerimientoObtenerUsuarioDTO:RequerimientoDTO
    {

        /// <summary>
        /// Clave de acceso al servicio
        /// </summary>
        [DataMember(IsRequired = true)]
        public string ClaveServicio { get; set; }

        /// <summary>
        /// Id del usuario al que se le realiza la consulta
        /// </summary>
        [DataMember(IsRequired = true)]
        public long UsuarioConsultaID { get; set; }


        /// <summary>
        /// Codigo de la estructura funcional de la aplicacion que realiza la consulta
        /// </summary>
        [DataMember(IsRequired = true)]
        public string EstructuraFuncional { get; set; }

    }
}