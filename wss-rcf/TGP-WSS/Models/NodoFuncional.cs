using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class NodoFuncional {
        public NodoFuncional() { }
        public virtual long CId { get; set; }
        [DisplayName("Estructura Funcional")]
        public virtual EstructuraFuncional EstructuraFuncional { get; set; }
        [DisplayName("Nodo Padre")]
        public virtual NodoFuncional NodoFuncionalPadre { get; set; }
        public virtual Usuario UsuarioLogin { get; set; }
        [DisplayName("Descripci�n")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string DDescripcion { get; set; }
        [DisplayName("C�digo")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string CNegocio { get; set; }
        [Required]
        public virtual int NVersionHibernate { get; set; }
        [DisplayName("Descentralizado")]
        public virtual short? MDescentralizado { get; set; }
        [DisplayName("�lt. Operaci�n")]
        public virtual DateTime? FUltimaOperacion { get; set; }
    }
}
