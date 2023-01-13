using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class ListadoNodoFuncionalViewModel: NodoFuncionalDTO
    {
        public string DescripcionNodoPadre { get; set; }
        public string DescripcionEstructura { get; set; }
        
    }
}
