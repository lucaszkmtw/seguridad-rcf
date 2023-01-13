using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto.Usuario
{
    public class UsuarioSigafDTO
    {
        public virtual string NombreUsuario { get; set; }

        public virtual bool? SiBloqueado { get; set; }

        public virtual DateTime? FechaBaja { get; set; }
    }
}
