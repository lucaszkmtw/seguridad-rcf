using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class EstructuraFuncional {

        public EstructuraFuncional() { }

        public virtual long CId { get; set; }

        [DisplayName("Usuario Login")]
        public virtual Usuario Usuario { get; set; }

        [DisplayName("Descripción")]
        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(160, MinimumLength = 2, ErrorMessage="El campo debe poseer entre 2 y 160 cáracteres.")]
        public virtual string DDescripcion { get; set; }

        [DisplayName("Código")]
        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El campo debe poseer entre 2 y 50 cáracteres.")]
        public virtual string CCodigo { get; set; }

        [Required]
        public virtual int NVersionHibernate { get; set; }

        [DisplayName("Últ. Operación")]
        public virtual DateTime? FUltimaOperacion { get; set; }

        [DisplayName("Bloqueado")]
        public virtual bool MBloqueado { get; set; }

        public virtual IList<NodoFuncional> Nodos { get; set; }
    }
}
