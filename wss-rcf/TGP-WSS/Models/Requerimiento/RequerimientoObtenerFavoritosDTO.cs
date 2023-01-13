using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TGP.WSS.Models.Requerimiento
{
    [DataContract]
    public class RequerimientoObtenerFavoritosDTO:RequerimientoDTO
    {
        /// <summary>
        /// Nombre de la clase a favoritear
        /// </summary>
        [DataMember(IsRequired = true)]
        public string NombreClase { get; set; }
       
    }
}