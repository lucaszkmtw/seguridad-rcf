using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS {

    public class Acreedor : Usuario
    {
        public Acreedor() { }

        [Required]
        [DisplayName("CUIT")]
        public virtual string DCuit { get; set; }
        [Required]
        [DisplayName("Razón Social")]
        public virtual string DRazonSocial { get; set; }

        public override String getDescripcionUsuario()
        {
            return DRazonSocial;
        }

        public override bool isAcreedor()
        {
            return true;
        }

        public override bool isNominado()
        {
            return false;
        }

        public override bool isInnominado()
        {
            return false;
        }
    }
}
