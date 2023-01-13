using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS
{
    public class UsuarioSIGAF
    {
        public virtual string CUser { get; set; }
        public virtual string XcUser { get; set; }
        public virtual int NDocumento { get; set; }
        public virtual string XlTelefono { get; set; }
        public virtual string XlEmail { get; set; }
        public virtual int? MSaf { get; set; }
        public virtual DateTime? FechaBaja { get; set; }
        public virtual bool? SiBloqueado { get; set; }
        public virtual DateTime? FechaBloqueo { get; set; }
    }
}