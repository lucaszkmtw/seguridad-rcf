using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class AuditoriaDeConexionesDTO
    {
        /// <summary>
        /// Id de la auditoria
        /// </summary>
        /// <summary>
        /// Usuario que ingreso
        /// </summary>
        public virtual string Usuario { get; set; }
        /// <summary>
        /// fecha de conexion
        /// </summary>
        public virtual DateTime FechaConexion { get; set; }
        /// <summary>
        /// Ip por la cual se conecto el usuario
        /// </summary>
        public virtual String IPConexion { get; set; }
        /// <summary>
        /// Navegador por el cual se conecto el usuario
        /// </summary>
        public virtual String Browser { get; set; }
        /// <summary>
        /// Servidor en el cual corre la aplicacion
        /// </summary>
        public virtual String Server { get; set; }
        /// <summary>
        /// Aplicacion que esta corriendo mediante la Estrutura Funcional
        /// </summary>
        public virtual String EstructuraFuncionalCodigo { get; set; }
        /// <summary>
        /// propiedad que se completa luego de la consulta de group by y almacena la cant de veces que se conecta el usuario en un dia
        /// </summary>
        public virtual int CantidadConexiones { get; set; }
    }
}
