using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TGP.WSS.Models.DTO
{
    /// <summary>
    /// Clase que modela al Usuario Nominado
    /// </summary>
    [DataContract]
    public class UsuarioNominadoDTO : UsuarioDTO
    {
        #region // Propiedades Publicas //

        /// <summary>
        /// Apellido del Usuario
        /// </summary>
        [DataMember]
        public string Apellidos { get; set; }

        /// <summary>
        /// Nombres del Usuario
        /// </summary>
        [DataMember]
        public string Nombres { get; set; }

        /// <summary>
        /// Numero de Documento del Usuario, hay casos que aqui se coloca el Nro de CUIL
        /// </summary>
        [DataMember]
        public string NumeroDocumento { get; set; }

        /// <summary>
        /// Codigo de Rol
        /// </summary>
        [DataMember]
        public string CodigoRol { get; set; }
        /// <summary>
        /// Cuit del usuario Nominado
        /// </summary>
        [DataMember]
        public string Cuit { get; set; }


        #endregion

    }
}