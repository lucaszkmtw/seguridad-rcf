using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.APIRequests
{
    /// <summary>
    /// Clase que modela un Menu para el mabiente de destino
    /// </summary>
    public class MenuDestino
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
        /// Id del menu padre si es q tiene
        /// </summary>
        public long? IdMenuOpcionPadre { get; set; }
        /// <summary>
        /// usuario que dio de alta el rol
        /// </summary>
        public string NombreUsuario{ get; set; }
        /// <summary>
        /// Codigo del rol
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Id de la actividad asociada si es que tiene
        /// </summary>
        public long? IdActividad { get; set; }

        /// <summary>
        /// Fecha de ultima operacion
        /// </summary>
        public DateTime? FechaUltimaOperacion { get; set; }
        /// <summary>
        /// Numero de orden del menu
        /// </summary>
        public short NumeroOrden { get; set; }
        /// <summary>
        /// Url del menu
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Icono del menu
        /// </summary>
        public string Icono { get; set; }
    }
}
