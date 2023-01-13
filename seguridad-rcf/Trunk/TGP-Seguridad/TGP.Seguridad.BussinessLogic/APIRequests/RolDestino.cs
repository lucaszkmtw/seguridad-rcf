using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.APIRequests
{
    public class RolDestino
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
        /// usuario que dio de alta el rol
        /// </summary>
        public string NombreUsuario{ get; set; }
        /// <summary>
        /// Codigo del rol
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Listado de Actividades asociadas al rol
        /// </summary>
        public IList<long> IdActividades { get; set; }

        /// <summary>
        /// Fecha de ultima operacion
        /// </summary>
        public DateTime? FechaUltimaOperacion { get; set; }
        /// <summary>
        /// Propiedad que indica si un rol es delegable (ADL)
        /// </summary>
        public bool SiDelegable { get; set; }
        /// <summary>
        /// Informacion detallada de la funcionalidad del rol
        /// </summary>
        public string InformacionRol { get; set; }
        /// <summary>
        /// Id Tipo de Nodos Funcionales que puede tener el Rol
        /// </summary>
        public long IdTipoNodoFuncional { get; set; }
        /// <summary>
        /// Rol es multinodo
        /// </summary>
        public int EsMultinodo { get; set; }
    }
}
