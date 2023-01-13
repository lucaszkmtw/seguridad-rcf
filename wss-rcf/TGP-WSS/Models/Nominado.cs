using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class Nominado : Usuario {   

        public Nominado() { }

        [Required]
        [DisplayName("DNI")]
        public virtual long NDni { get; set; }
        [Required]
        [DisplayName("Apellido")]
        public virtual string DApellido { get; set; }
        [Required]
        [DisplayName("Nombre")]
        public virtual string DNombre { get; set; }
        [DisplayName("Cuit")]
        public virtual string Cuit { get; set; }

        public override String getDescripcionUsuario()
        {
            return DNombre+" "+DApellido;
        }

        public override bool isAcreedor()
        {   
            return false;
        }

        public override bool isNominado()
        {   
            return true;
        }

        public override bool isInnominado()
        {
            return false;
        }
    }
}
