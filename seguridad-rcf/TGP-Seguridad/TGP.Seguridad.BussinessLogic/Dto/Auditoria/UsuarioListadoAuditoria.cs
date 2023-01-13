using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class UsuarioListadoAuditoria: AuditoriaDeConexionesDTO
    {
        public UsuarioListadoAuditoria()
        {
            this.AplicacionesAccedidas = new List<string>();
        }
        public long IdUsuario { get; set; }
        public string TipoUsuario { get; set; }
        public string CodigoTipoUsuario { get; set; }
        public DateTime UltimaConexion { get; set; }
        public IList<string> AplicacionesAccedidas { get; set; }
    }
}
