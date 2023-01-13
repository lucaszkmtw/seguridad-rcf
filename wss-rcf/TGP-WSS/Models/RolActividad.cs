using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class RolActividad {
        public virtual Rol Rol { get; set; }
        public virtual Actividad Actividad { get; set; }
        public virtual Usuario UsuarioLogin { get; set; }
        public virtual DateTime? FUltimaOperacion { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as RolActividad;
			if (t == null) return false;
			if (Rol.CId == t.Rol.CId
			 && Actividad.CId == t.Actividad.CId)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ Rol.CId.GetHashCode();
			hash = (hash * 397) ^ Actividad.CId.GetHashCode();

			return hash;
        }
        #endregion
    }
}
