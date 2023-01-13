using System;

namespace TGP.Seguridad.DataAccess.Generics
{
    /// <summary>
    /// Clase que se utiliza para auditar, las clase que necesiten auditar deben derivar de ella
    /// y contemplar contener en su tabla la siguiente info
    /// </summary>
    public class Audit
    {
        #region // Propiedades Publicas //
        public virtual DateTime FechaAlta { get; set; }

        public virtual DateTime FechaActualizacion { get; set; }

        public virtual DateTime? FechaBaja { get; set; }

        public virtual string UsuarioAlta { get; set; }

        public virtual string UsuarioActualizacion { get; set; }

        public virtual string UsuarioBaja { get; set; }

        public virtual int Version { get; set; }

        public virtual bool Activado { get; set; }
        #endregion

    }
}
