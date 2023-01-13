using System;
using System.Text;
using System.Collections.Generic;

namespace TGP.WSS
{

    public class Solicitud {
        public Solicitud() { }
        public virtual long Id { get; set; }
        public virtual EstadoSolicitud EstadoSolicitud { get; set; }
        public virtual long Cuit { get; set; }
        public virtual string Apellido { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Email { get; set; }
        public virtual string CodigoValidacion { get; set; }
        public virtual string Telefono { get; set; }
        public virtual DateTime FechaAlta { get; set; }
        public virtual TipoSolicitud TipoSolicitud { get; set; }
        public virtual string Cargo { get; set; }
        public virtual bool MEnviado { get; set; }
        public virtual int VersionHibernate { get; set; }
    }
}
