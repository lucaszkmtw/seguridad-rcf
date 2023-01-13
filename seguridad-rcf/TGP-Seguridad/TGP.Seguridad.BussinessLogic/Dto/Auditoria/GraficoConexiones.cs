using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class GraficoConexiones
    {
        public virtual string Nominados { get; set; }

        public virtual string Acreedores { get; set; }

        public virtual long MaxConexiones { get; set; }
    }
}
