using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TGP.WSS.Models.DTO
{
    /// <summary>
    /// Clase que modela el usuario de seguridad en DTO
    /// </summary>
    [DataContract]
    public class UsuarioDTO
    {
        #region // Propiedades Publicas //

        /// <summary>
        /// Nro de identificacion unica del usuario
        /// </summary>
        [DataMember]
        public Int64 ID { get; set; }

        /// <summary>
        /// Nombre de Usuario
        /// </summary>
        [DataMember]
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Emial del Usuario
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Telefono del Usuario
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// Imagen o Avatar del usuario
        /// </summary>
        [DataMember]
        public Byte[] Avatar { get; set; }

        #endregion

    }
}