using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class MenuOpcionMap : ClassMap<MenuOpcion>
    {
        public MenuOpcionMap()
        {
            Table("SEG_MENU_OPCION");
            Id(x => x.Id, "C_ID").GeneratedBy.Assigned();
            References(x => x.EstructuraFuncional, "C_ID_ESTRUCTURA_FUNCIONAL").Class(typeof(EstructuraFuncional)).Fetch.Join().Cascade.None().NotFound.Ignore();
            References(x => x.UsuarioResponable, "C_ID_USUARIO_LOGIN").Fetch.Join().Class(typeof(Usuario)).Cascade.None();
            References(x => x.MenuOpcionPadre, "C_ID_PADRE").Fetch.Join().Class(typeof(MenuOpcion)).Cascade.None();
            References(x => x.ActividadAsociada).Column("C_ID").PropertyRef(x=>x.MatchMenuOpcion).Class(typeof(ActividadMenuOpcion)).Cascade.All().Not.Insert().Not.Update().NotFound.Ignore();
            Map(x => x.Descripcion, "D_DESCRIPCION");
            Map(x => x.Codigo, "C_CODIGO");
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            Map(x => x.Icono, "D_ICONO");
            Map(x => x.NumeroOrden, "N_ORDEN");
            Map(x => x.Url, "D_URL");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
    }

    public class MenuOpcion: BaseEntity
    {
        #region // Propiedades Publicas
        public virtual MenuOpcion MenuOpcionPadre { get; set; }
        public virtual EstructuraFuncional EstructuraFuncional { get; set; }
        public virtual Usuario UsuarioResponable { get; set; }
        public virtual string Codigo { get; set; }
        public virtual short NumeroOrden { get; set; }
        public virtual string Url { get; set; }
        public virtual string Icono { get; set; }
        public virtual DateTime? FechaUltimaOperacion { get; set; }
        public virtual ActividadMenuOpcion ActividadAsociada { get; set; }

        #endregion

    }
}
