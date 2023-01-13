using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{

    public class AuditoriaConexionMap : ClassMap<AuditoriaConexion>
    {

        public AuditoriaConexionMap()
        {
            Table("SEG_AUDITORIA_CONEXION");
            Id(x => x.Id, "C_ID").GeneratedBy.SequenceIdentity("SEG_AUDITORIA_CONEXION_SQ");
            References(x => x.Usuario, "C_ID_USUARIO").Fetch.Join().Class(typeof(Usuario)).Cascade.None();
            Map(x => x.FechaConexion, "F_CONEXION");
            Map(x => x.IPConexion, "D_IP");
            Map(x => x.Browser, "D_BROWSER");
            Map(x => x.Server, "D_SERVER");
            Map(x => x.EstructuraFuncionalCodigo, "C_ESTRUCTURA_FUNCIONAL");

            Map(x => x.EstructuraFuncionalDescripcion).Formula(@"(select n.d_descripcion from seg_estructura_funcional n where n.c_codigo = C_ESTRUCTURA_FUNCIONAL)");

        }

    }
    public class AuditoriaConexion: BaseEntity
    {
        /// <summary>
        /// Id de la auditoria
        /// </summary>
        /// <summary>
        /// Usuario que ingreso
        /// </summary>
        public virtual Usuario Usuario { get; set; }
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


        /// <summary>
        /// 
        /// </summary>
        public virtual string EstructuraFuncionalDescripcion { get; set; }

    }
}
