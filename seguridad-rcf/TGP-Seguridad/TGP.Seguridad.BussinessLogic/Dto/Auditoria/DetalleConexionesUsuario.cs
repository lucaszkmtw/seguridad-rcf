using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class DetalleConexionesUsuario
    {
        public string NombreUsuario { get; set; }
        public string DescripcionUsuario { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public IList<AuditoriaDeConexionesDTO> Conexiones { get; set; }
    }
}
