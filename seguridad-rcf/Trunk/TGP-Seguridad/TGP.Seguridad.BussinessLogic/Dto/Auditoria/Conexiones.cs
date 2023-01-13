using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class Conexiones
    {
        public Conexiones()
        {
            this.GraficosConexion = new GraficosConexionesFecha();
            this.GraficoConexionesDiarias = new GraficoConexionesHora();
        }
        public virtual GraficosConexionesFecha GraficosConexion { get; set; }
        public virtual GraficoConexionesHora GraficoConexionesDiarias { get; set; }

        public long CantidadNominados { get; set; }
        public long CantidadAcreedores { get; set; }
        public long CantidadNominadosActivos { get; set; }
        public long CantidadAcreedoresActivos { get; set; }




    }
}
