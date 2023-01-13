using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class AuditRolNodoUsuario {
        public virtual long CId { get; set; }
        [Required]
        public virtual long CRol { get; set; }
        [Required]
        public virtual string DRol { get; set; }
        [Required]
        public virtual long CNodoFuncional { get; set; }
        [Required]
        public virtual string DNodoFuncional { get; set; }
        [Required]
        public virtual string CUsuario { get; set; }
        [Required]
        public virtual DateTime FOperacion { get; set; }
        [Required]
        public virtual string DOperacion { get; set; }
        [Required]
        public virtual string CUsuarioLogin { get; set; }
    }
}
