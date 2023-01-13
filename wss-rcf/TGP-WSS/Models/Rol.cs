using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class Rol {
        public Rol() { }
        public virtual long CId { get; set; }
        [DisplayName("Estructura Funcional")]
        public virtual EstructuraFuncional EstructuraFuncional { get; set; }
        public virtual Usuario Usuario { get; set; }
        [DisplayName("Descripción")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string DDescripcion { get; set; }
        [DisplayName("Codigo")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string Codigo { get; set; }
        [Required]
        public virtual int NVersionHibernate { get; set; }
        [DisplayName("Últ. Operación")]
        public virtual DateTime? FUltimaOperacion { get; set; }
        
        public virtual string DescripcionEstructura{
          get{
             return "("+this.EstructuraFuncional.CCodigo+") " + this.DDescripcion;
          }
        }

    }
}
