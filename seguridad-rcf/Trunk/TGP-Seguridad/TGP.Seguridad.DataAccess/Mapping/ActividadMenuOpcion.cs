using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using TGP.Seguridad.DataAccess.Generics;
using TGPCommonAnnotationsLibrary.Common;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class ActividadMenuOpcionMap : ClassMap<ActividadMenuOpcion>
    {
        public ActividadMenuOpcionMap()
        {
            Table("SEG_ACTIVIDAD_MENU_OPCION");
            CompositeId()
                .KeyProperty(x => x.IdActividad, "C_ID_ACTIVIDAD")
                .KeyProperty(x => x.IdMenuOpcion, "C_ID_MENU_OPCION");

            References(x => x.UsuarioAlta, "C_ID_USUARIO_LOGIN").Class(typeof(Usuario));
            References(x => x.Actividad, "C_ID_ACTIVIDAD").Class(typeof(Actividad)).Not.Insert().Not.Update().Cascade.None();
            References(x => x.MenuOpcion, "C_ID_MENU_OPCION").Class(typeof(MenuOpcion)).Not.Insert().Not.Update().Cascade.None();
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            Map(x => x.MatchMenuOpcion, "C_ID_MENU_OPCION").Not.Insert().Not.Update();
        }
    }

    public class ActividadMenuOpcion : BaseEntity
    {

        #region // Propiedades Publicas //
        [IsId]
        public virtual long IdMenuOpcion { get; set; }
        [IsId]
        public virtual long IdActividad { get; set; }
        public virtual long MatchMenuOpcion { get; set; }
        /// <summary>
        /// Actividad 
        /// </summary>
        public virtual Actividad Actividad { get; set; }
        /// <summary>
        /// Menu
        /// </summary>
        
        public virtual MenuOpcion MenuOpcion { get; set; }
        
        /// <summary>
        /// Usuario que dio de alta la relacion
        /// </summary>
        public virtual Usuario UsuarioAlta { get; set; }
        /// <summary>
        /// Fecha de ultima operacion
        /// </summary>
        public virtual DateTime? FechaUltimaOperacion { get; set; }
        #endregion

        #region NHibernate Composite Key Requirements

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as ActividadMenuOpcion;
            if (t == null) return false;
            if (IdActividad == t.IdActividad
             && IdMenuOpcion == t.IdMenuOpcion)
                return true;

            return false;
        }
        public override int GetHashCode()
        {
            int hash = GetType().GetHashCode();
            hash = (hash * 397) ^ IdActividad.GetHashCode();
            hash = (hash * 397) ^ IdMenuOpcion.GetHashCode();

            return hash;
        }
        #endregion

    }
}
