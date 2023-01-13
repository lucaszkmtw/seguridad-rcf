using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class ComboGenerico
    {
        public const string ComboVacio = "0";

        public long Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
}
