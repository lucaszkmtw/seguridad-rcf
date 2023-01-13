using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class UsuarioAutoComplete
    {
        public long Id { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreApellidoRazonSocial { get; set; }
        public string DescripcionAsignacionMasiva { get; set; }
    }
}
