using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Models.Resultado
{
    public class ResultadoObtenerUsuariosNominadosPorRolDTO:ResultadoDTO
    {
        /// <summary>
        /// Lista de usuarios nominados
        /// </summary>
        public List<DTO.UsuarioNominadoDTO> UsuariosNominadosDTO { get; set; }
    }
}