using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.APIRequests
{
    public class ActividadDestino
    {
        /// <summary>
        /// Propiedad que modela la columna ID 
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Propiedad que modela la columna Descripcion
        /// </summary>
        public string Descripcion { get; set; }
        /// <summary>
        /// Id Estructura funcional 
        /// </summary>
        public long IdEstructuraFuncional { get; set; }
        /// <summary>
        /// usuario que dio de alta la actividad
        /// </summary>
        public string NombreUsuario{ get; set; }
        /// <summary>
        /// Codigo de la actividad
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Fecha de ultima operacion
        /// </summary>
        public DateTime? FechaUltimaOperacion { get; set; }
    }
}
