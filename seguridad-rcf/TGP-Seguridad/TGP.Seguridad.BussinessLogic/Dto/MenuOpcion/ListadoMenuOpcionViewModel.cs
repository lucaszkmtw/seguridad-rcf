using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class ListadoMenuOpcionViewModel : MenuOpcionDTO
    {
        public string CodigoEstructura { get; set; }
        public string DescripcionEstructura { get; set; }
        public string MenuPadre { get; set; }
    }
}
