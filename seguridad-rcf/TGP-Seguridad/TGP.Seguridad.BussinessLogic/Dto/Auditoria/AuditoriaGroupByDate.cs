using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class AuditoriaGroupByDate
    {
        public DateTime Fecha { get; set; }

        public int Hora { get; set; }

        public long Conexiones { get; set; }

    }
}
