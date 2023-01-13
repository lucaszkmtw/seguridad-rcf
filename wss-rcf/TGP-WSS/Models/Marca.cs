using System;
using System.Text;
using System.Collections.Generic;


namespace TGP.WSS {
    
    public class Marca {
        public virtual long Id { get; set; }
        public virtual NombreClase NombreClase { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual DateTime FechaRegistro { get; set; }
        public virtual long ObjetoId { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string ColorFondo { get; set; }
        public virtual string ColorTexto { get; set; }
        public virtual long VersionHibernate { get; set; }
    }
}
