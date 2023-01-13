using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class AdminsLocalesDeUsuarioEstructuraViewModel
    {
        public long Id { get; set; }
        public string Estructura { get; set; }
        public List<AdministradorLocalDTO> AdministradoresLocales { get; set; }
    }
}
