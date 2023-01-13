using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TGP.WSS.Models.Resultado
{
    [DataContract]
    public class ResultadoObtenerFavoritosDTO:ResultadoDTO
    {
        private List<FavoritoDTO> favoritos = new List<FavoritoDTO>();

        [DataMember]
        public List<FavoritoDTO> Favoritos {
            get { return favoritos; }
            set { favoritos = value; }
        }
    }

    [DataContract]
    public class FavoritoDTO
    {
        [DataMember]
        public long ObjetoID { get; set; }
        [DataMember]
        public string NombreClase { get; set; }

    }
}