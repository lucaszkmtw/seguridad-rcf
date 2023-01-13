using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class TipoSolicitud {
        public TipoSolicitud() { }


        public virtual long CId { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
    }
}
