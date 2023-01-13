using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class TipoUsuario {
        public TipoUsuario() { }

        public static string INNOMINADO = "I";

        public static string NOMINADO = "N";

        public static string ACREEDOR = "A";

        public virtual long CId { get; set; }
        [Required]
        public virtual string CCodigo { get; set; }
        [Required]
        public virtual string DDescripcion { get; set; }
        public virtual int NVersionHibernate { get; set; }
    }
}
