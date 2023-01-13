using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS {

    public class TokenSesionMobile
    {
        public virtual long Id { get; set; }
        public virtual long UsuarioID { get; set; }
        public virtual string Token { get; set; }
        public virtual DateTime? FechaRegistro { get; set; }
        public virtual DateTime FechaExpiracion { get; set; }
        public virtual int NVersionHibernate { get; set; }
        public virtual DateTime? FechaAlta { get; set; }
        public virtual DateTime? FechaBaja { get; set; }
    }
}
