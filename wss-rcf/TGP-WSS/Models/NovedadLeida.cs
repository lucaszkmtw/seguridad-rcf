using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class NovedadLeida {
        public virtual Usuario Usuario { get; set; }
        public virtual Novedad Novedad { get; set; }
        [Required]
        public virtual DateTime FLectura { get; set; }
        public virtual int NVersionHibernate { get; set; }
        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj) {
			if (obj == null) return false;
			var t = obj as NovedadLeida;
			if (t == null) return false;
			if (Usuario.CId == t.Usuario.CId
			 && Novedad.CId == t.Novedad.CId)
				return true;

			return false;
        }
        public override int GetHashCode() {
			int hash = GetType().GetHashCode();
			hash = (hash * 397) ^ Usuario.CId.GetHashCode();
			hash = (hash * 397) ^ Novedad.CId.GetHashCode();

			return hash;
        }
        #endregion
    }
}
