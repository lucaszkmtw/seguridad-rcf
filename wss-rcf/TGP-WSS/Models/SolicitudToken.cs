using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS {

    public class SolicitudToken
    {
        public SolicitudToken() { }
        public virtual long CId { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Valor { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string TipoDato { get; set; }
        public virtual string FormatoTransf { get; set; }
        public virtual string Usuario { get; set; }
        public virtual DateTime FechaAlta { get; set; }
        public virtual DateTime FechaModificacion { get; set; }
    }
}
