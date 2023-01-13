using System;
using System.Text;
using System.Collections.Generic;


namespace TGP.WSS
{
    
    public class SolicitudNodo {
        public virtual long Id { get; set; }
        public virtual Solicitud Solicitud { get; set; }
        public virtual NodoFuncional NodoFuncional { get; set; }
        public virtual DateTime FechaAlta { get; set; }
        public virtual int VersionHibernate { get; set; }
    }
}
