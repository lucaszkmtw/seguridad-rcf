using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class PermisosViewModel
    {
        public long IdRol { get; set; }
        public long IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string UsuarioAlta { get; set; }
        public string DescripcionRol { get; set; }
        public string CodigoRol { get; set; }
        public string DescripcionEstructura { get; set; }
        public string CodigoEstructura { get; set; }
        public IList<string> NodosAsignados { get; set; }
        public IList<string> CodigoNodosAsignados { get; set; }
        public DateTime FechaUltimaOperacion { get; set; }
    }
}
