using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TGP.WSS.Models.Requerimiento
{
    /// <summary>
    /// Clase que modela el requerimiento del servicio de Generar Usuario Nominado
    /// </summary>
    [DataContract]
    public class RequerimientoGenerarUsuarioNominadoDTO : RequerimientoDTO
    {
        #region // Propiedades Publicas //

        /// <summary>
        /// Codigo de la Estructura Funcional en la cual se va a dar de alta al usuario
        /// </summary>
        [DataMember(IsRequired = true)]
        public string CodigoEstructuraFuncional { get; set; }

        /// <summary>
        /// ID de la solictud en la cual se cargo el usuario para ser evaluado
        /// y luego dado de alta mediante este servicio
        /// </summary>
        [DataMember(IsRequired = false)]
        public int SolicitudID { get; set; }

        /// <summary>
        /// Codigo del ROL al cual se le asignarl al usuario
        /// </summary>
        [DataMember(IsRequired = true)]
        public string CodigoRol { get; set; }

        /// <summary>
        /// Usuario a dar de alta en el sistema
        /// </summary>
        [DataMember(IsRequired = true)]
        public DTO.UsuarioNominadoDTO UsuarioNominadoDTO { get; set; }

        #endregion
    }
}