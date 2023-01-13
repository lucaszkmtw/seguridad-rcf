using System.Runtime.Serialization;

namespace TGP.WSS.Models.Requerimiento
{
    /// <summary>
    /// Clase que modela el requerimiento base de los servicios
    /// </summary>
    [DataContract]
    public class RequerimientoDTO
    {
        /// <summary>
        /// Nombre de usuario de acceso
        /// </summary>
        [DataMember(IsRequired = true)]
        public string NombreUsuario { get; set; }
    }
}