using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class ActividadMenuOpcion {
        [DisplayName("Actividad")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual Actividad Actividad { get; set; }
        [DisplayName("Opción de Menú")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual MenuOpcion MenuOpcion { get; set; }
        public virtual Usuario UsuarioLogin { get; set; }
        public virtual DateTime? FUltimaOperacion { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as ActividadMenuOpcion;
			if (t == null) return false;
			if (Actividad.CId == t.Actividad.CId
			 && MenuOpcion.CId == t.MenuOpcion.CId)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ Actividad.CId.GetHashCode();
			hash = (hash * 397) ^ MenuOpcion.CId.GetHashCode();

			return hash;
        }
        #endregion
    }
}
