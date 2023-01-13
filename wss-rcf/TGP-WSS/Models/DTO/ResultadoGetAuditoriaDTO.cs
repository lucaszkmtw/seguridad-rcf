using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Models.DTO
{
    public class ResultadoGetAuditoriaDTO : Resultado.ResultadoDTO
    {
        /// <summary>
        /// Listado de Registros de la Auditoria
        /// </summary>
        public List<AuditoriaDTO> Items{ get; set; }

    }
}