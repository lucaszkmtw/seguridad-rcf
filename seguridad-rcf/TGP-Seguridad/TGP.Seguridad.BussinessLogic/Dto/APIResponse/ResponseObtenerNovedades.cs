using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto.APIResponse
{
    public class ResponseObtenerNovedades : ResponseAPI
    {
        /// <summary>
        /// 
        /// </summary>
        public List<NovedadDTO> NovedadesNoLeidas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<NovedadDTO> AdvertenciasTotalesVigentes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<NovedadDTO> AdvertenciasNoLeidas { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<NovedadDTO> NovedadesVigentes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<NovedadDTO> NovedadesGuardadas { get; set; }
    }
}
