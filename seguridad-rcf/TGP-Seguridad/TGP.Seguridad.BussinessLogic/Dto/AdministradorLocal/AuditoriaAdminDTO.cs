using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class AuditoriaAdminDTO
    {
        public virtual string DescripcionRol { get; set; }
        public virtual string DescripcionNodoFuncional { get; set; }
        public virtual string NombreUsuario { get; set; }
        public virtual DateTime FechaOperacion { get; set; }
        public virtual string DescripcionOperacion { get; set; }
        public virtual string DescripcionEstructura { get; set; }
    }
}
