using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.APIRequests
{
    public class NodoDestino
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
        /// Id Nodo FuncionalPadre 
        /// </summary>
        public long? IdNodoPadre { get; set; }
        /// <summary>
        /// usuario que dio de alta el rol
        /// </summary>
        public string NombreUsuario{ get; set; }
        /// <summary>
        /// Codigo del rol
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Fecha de ultima operacion
        /// </summary>
        public DateTime? FechaUltimaOperacion { get; set; }
        /// <summary>       
        /// <summary>
        /// Marca que determina si el Organismo es descentralizado o no
        /// </summary>
        public virtual bool SiDescentralizado { get; set; }
        /// <summary>
        /// Id Tipo de Nodo Funcional
        /// </summary>
        public long IdTipoNodoFuncional { get; set; }
    }
}
