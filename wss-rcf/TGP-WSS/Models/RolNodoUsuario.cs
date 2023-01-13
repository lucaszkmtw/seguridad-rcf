using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class RolNodoUsuario {
        public virtual long CId { get; set; }
        public virtual Rol Rol { get; set; }
        public virtual NodoFuncional NodoFuncional { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario UsuarioLogin { get; set; }
        public virtual int NVersionHibernate { get; set; }
        public virtual DateTime? FUltimaOperacion { get; set; }
    }
}
