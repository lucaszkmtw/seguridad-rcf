using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class AdministradorLocalDTO
    {
        public virtual UsuarioDTO UsuarioAdmin { get; set; }
        public virtual NodoFuncionalDTO NodoFuncional { get; set; }
        public List<RolDelegadoAdminDTO> RolesDelegadosAdmins { get; set; }
    }
}
