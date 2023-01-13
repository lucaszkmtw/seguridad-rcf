using FluentNHibernate.Mapping;
using System;
using TGP.Seguridad.DataAccess.Generics;
using TGPCommonAnnotationsLibrary.Common;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class RolActividadMap : ClassMap<RolActividad>
    {
        public RolActividadMap()
        {
            Table("SEG_ROL_ACTIVIDAD");
            CompositeId()
                .KeyProperty(x => x.IdRol, "C_ID_ROL")
                .KeyProperty(x => x.IdActividad, "C_ID_ACTIVIDAD");
            References(x => x.UsuarioResponsable, "C_ID_USUARIO_LOGIN").Class(typeof(Usuario)).Cascade.None();
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            References(x => x.Actividad, "C_ID_ACTIVIDAD").Class(typeof(Actividad)).Not.Insert().Not.Update();
            References(x => x.Rol, "C_ID_ROL").Class(typeof(Rol)).Not.Insert().Not.Update();
        }
    }

    public class RolActividad : BaseEntity
    {
        #region // Propiedades Publicas //
        [IsId]
        public virtual long IdRol { get; set; }
        [IsId]
        public virtual long IdActividad { get; set; }
        
        public virtual Rol Rol { get; set; }
        
        public virtual Actividad Actividad { get; set; }
        public virtual Usuario UsuarioResponsable { get; set; }
        public virtual DateTime? FechaUltimaOperacion { get; set; }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as RolActividad;
            if (t == null) return false;
            if (IdRol == t.IdRol
             && IdActividad== t.IdActividad)
                return true;

            return false;
        }
        public override int GetHashCode()
        {
            int hash = GetType().GetHashCode();
            hash = (hash * 397) ^ IdRol.GetHashCode();
            hash = (hash * 397) ^ IdActividad.GetHashCode();

            return hash;
        }
    }
}
