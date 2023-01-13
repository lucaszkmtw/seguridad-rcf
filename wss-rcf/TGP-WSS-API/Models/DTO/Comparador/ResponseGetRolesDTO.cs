
using System.Collections.Generic;
using TGP.WSS;

namespace TGP.WSS.API.Models.DTO.Comparador
{
    public class ResponseGetRolesDTO : ResponseGetEntidadesDTO
    {
        public ResponseGetRolesDTO()
        {
            Roles = new List<ElementoComparar>();
        }
        /// <summary>
        /// Roles encontrados en el destino
        /// </summary>
        public IList<ElementoComparar> Roles { get; set; }


    }
}