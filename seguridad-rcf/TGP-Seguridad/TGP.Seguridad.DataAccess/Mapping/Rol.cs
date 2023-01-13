using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class RolMap : ClassMap<Rol>
    {
        public RolMap()
        {
            Table("SEG_ROL");
            Id(x => x.Id, "C_ID").GeneratedBy.Assigned();
            References(x => x.EstructuraFuncional, "C_ID_ESTRUCTURA_FUNCIONAL").Class(typeof(EstructuraFuncional)).Cascade.None();
            References(x => x.UsuarioAlta, "C_ID_USUARIO_LOGIN").Class(typeof(Usuario)).Cascade.None();
            References(x => x.TipoNodoFuncional, "C_ID_TIPO_NODO").Class(typeof(TipoNodoFuncional)).Cascade.None();
            Map(x => x.Descripcion, "D_DESCRIPCION");
            Map(x => x.Codigo, "C_CODIGO");
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            Map(x => x.InformacionRol, "D_INFORMACION");
            Map(x => x.EsMultinodo, "M_MULTINODO");
            HasMany(x => x.Actividades).KeyColumn("C_ID_ROL").Inverse().Cascade.AllDeleteOrphan();
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
            Map(x => x.SiDelegable).Column("M_DELEGABLE");
        }
    }

    public class Rol: BaseEntity
    {
        #region // Propiedades Publicas
        /// <summary>
        /// Estructura funcional 
        /// </summary>
        public virtual EstructuraFuncional EstructuraFuncional { get; set; }
        /// <summary>
        /// usuario que dio de alta el rol
        /// </summary>
        public virtual Usuario UsuarioAlta { get; set; }
        /// <summary>
        /// Codigo del usuario
        /// </summary>
        public virtual string Codigo { get; set; }
        
        /// <summary>
        /// Listado de Actividades asociadas al rol
        /// </summary>
        public virtual IList<RolActividad> Actividades { get; set; }

        /// <summary>
        /// Fecha de ultima operacion
        /// </summary>
        public virtual DateTime? FechaUltimaOperacion { get; set; }
        public virtual bool SiDelegable { get; set; }

        public virtual string InformacionRol { get; set; }
        public virtual int EsMultinodo { get; set; }
        public virtual TipoNodoFuncional TipoNodoFuncional { get; set; }

        #endregion

    }
}
