using System.Runtime.Serialization;

namespace TGP.WSS.Models.Requerimiento
{
    /// <summary>
    /// Clase que modela el requerimiento de marcar como favorito a un objeto del sistema
    /// </summary>
    [DataContract]
    public class RequerimientoFavoritearDTO:RequerimientoDTO
    {
        /// <summary>
        /// Nombre de la clase a favoritear
        /// </summary>
        [DataMember(IsRequired = true)]
        public string NombreClase { get; set; }
        /// <summary>
        /// Id del objeto a favoritear
        /// </summary>
        [DataMember(IsRequired = true)]
        public int ObjetoID { get; set; }

    }
}