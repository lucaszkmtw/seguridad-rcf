using System;
using System.Text;
using System.Collections.Generic;


namespace TGP.WSS {
    
    public class NombreClase {
        public NombreClase() { }
        public virtual long Id { get; set; }
        public virtual string Nombre { get; set; }
        public virtual long VersionHibernate { get; set; }
    }
}
