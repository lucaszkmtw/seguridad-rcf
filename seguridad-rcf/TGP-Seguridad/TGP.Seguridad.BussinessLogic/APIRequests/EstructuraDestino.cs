using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.APIRequests
{
    public class EstructuraDestino
    {
        /// <summary>
        /// Propiedad que modela la columna ID 
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Propiedad que modela la columna Descripcion
        /// </summary>
        public string DescripcionEstructura { get; set; }
        /// <summary>
        /// usuario que dio de alta el rol
        /// </summary>
        public string NombreUsuario{ get; set; }
        /// <summary>
        /// Codigo de la estructura
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Fecha de ultima operacion
        /// </summary>
        public DateTime? FechaUltimaOperacion { get; set; }
        /// <summary>
        /// Marca que determina si la estrutura esta bloquedad
        /// </summary>
        public virtual bool SiBloqueado { get; set; }
    }
}
