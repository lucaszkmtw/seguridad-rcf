using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class RolDelegadoAdminDTO
    {
        public virtual RolDTO Rol { get; set; }
        public virtual UsuarioDTO UsuarioAdminLocal { get; set; }
    }
}
