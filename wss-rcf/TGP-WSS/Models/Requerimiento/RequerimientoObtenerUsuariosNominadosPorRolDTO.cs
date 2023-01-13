using System.Runtime.Serialization;


namespace TGP.WSS.Models.Requerimiento
{
    [DataContract]
    public class RequerimientoObtenerUsuariosNominadosPorRolDTO:RequerimientoDTO
    {
        /// <summary>
        /// Codigo del Rol
        /// </summary>
        [DataMember(IsRequired = true)]
        public string CodigoRol { get; set; }

        /// <summary>
        /// Clave de acceso al servicio
        /// </summary>
        [DataMember(IsRequired = true)]
        public string ClaveServicio { get; set; }

        /// <summary>
        /// Codigo de la estructura funcional de la aplicacion que realiza la consulta
        /// </summary>
        [DataMember(IsRequired = true)]
        public string EstructuraFuncional { get; set; }

    }
}