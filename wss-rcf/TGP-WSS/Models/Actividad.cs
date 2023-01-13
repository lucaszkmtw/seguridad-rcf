using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS{
    
    public class Actividad {
        public Actividad() { }

        public virtual long CId { get; set; }

        [DisplayName("Estructura Funcional")]        
        public virtual EstructuraFuncional EstructuraFuncional { get; set; }

        public virtual Usuario Usuario { get; set; }

        [DisplayName("Código")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string CCodigo { get; set; }  
      
        [DisplayName("Descripción")]
        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(160, MinimumLength = 2, ErrorMessage = "El campo debe poseer entre 2 y 160 cáracteres.")]
        public virtual string DDescripcion { get; set; } 
                
        [DisplayName("Últ. Operación")]
        public virtual DateTime? FUltimaOperacion { get; set; }

        public virtual IList<MenuOpcion> Menues { get; set; }

        public virtual int NVersionHibernate { get; set; }

    }
}
