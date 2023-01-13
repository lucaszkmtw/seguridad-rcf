using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class ListadoAdministradoresLocalesViewModel
    {
        public long Id { get; set; }
        public string NombreUsuario { get; set; }
        public string DescripcionUsuario { get; set; }
        public List<AdminLocalLivianoDTO> AdministradoresLocalesDTO { get; set; }
        public List<string> Estructuras { get; set; }
    }
}
