using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{

    public class Innominado : Usuario
    {
        public Innominado() { }

        [Required]
        [DisplayName("Denominación")]
        public virtual string DDescripcion { get; set; }

        public override String getDescripcionUsuario()
        {
            return DDescripcion;
        }

        public override bool isAcreedor()
        {
            return false;
        }

        public override bool isNominado()
        {
            return false;
        }

        public override bool isInnominado()
        {
            return true;
        }
    }
}
