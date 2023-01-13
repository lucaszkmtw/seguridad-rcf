using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class VWUsuarioPermisos {

        public VWUsuarioPermisos() { }
               
        public virtual TipoAutenticacion TipoAutenticacion { get; set; }
        
        public virtual string DUsuario { get; set; }

        public virtual string DPassword { get; set; }        
       
        public virtual short MActivo { get; set; }        
        public virtual short MBloqueado { get; set; }

        public virtual string DRol { get; set; }
        /// <summary>
        /// Codigo del Rol del usuario
        /// </summary>
        public virtual string CodigoRol { get; set; }

        public virtual string CNodoFuncional { get; set; }

        public virtual string DNodoFuncional { get; set; }

        public virtual string DEstructuraFuncional { get; set; }

        public virtual string CEstructuraFuncional { get; set; }

        public virtual EstructuraFuncional EstructuraFuncional { get; set; }

        public virtual string DActividad { get; set; }

        public virtual MenuOpcion MenuOpcion  { get; set; }

        public virtual string DMenuOpcion { get; set; }

        public virtual MenuOpcion MenuOpcionPadre { get; set; }

        public virtual Rol Rol { get; set; }

        public virtual long NOrden { get; set; }

        public virtual string DUrl { get; set; }

        public virtual string DIcono { get; set; }

        #region NHibernate Composite Key Requirements
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as VWUsuarioPermisos;
            if (t == null) return false;
            if (EstructuraFuncional.CId == t.EstructuraFuncional.CId
             && DUsuario == t.DUsuario && DActividad == t.DActividad && DNodoFuncional==t.DNodoFuncional)
                return true;

            return false;
        }
        public override int GetHashCode()
        {
            int hash = GetType().GetHashCode();
            hash = (hash * 397) ^ EstructuraFuncional.CId.GetHashCode();
            hash = (hash * 397) ^ DNodoFuncional.GetHashCode();
            hash = (hash * 397) ^ DUsuario.GetHashCode();
            hash = (hash * 397) ^ DActividad.GetHashCode();

            return hash;
        }
        #endregion
    }
}
