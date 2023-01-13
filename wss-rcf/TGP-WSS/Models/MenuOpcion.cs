using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public class MenuOpcion {
        public MenuOpcion() {
            this.Actividades = new List<Actividad>();
        }
        public virtual int CId { get; set; }
        [DisplayName("Menú Padre")]        
        public virtual MenuOpcion MenuOpcionPadre { get; set; }
        [DisplayName("Estructura Funcional"), Required]       
        public virtual EstructuraFuncional EstructuraFuncional { get; set; }
        public virtual Usuario UsuarioLogin { get; set; }
        [DisplayName("Código")]
        [StringLength(5)]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string CCodigo { get; set; }
        [DisplayName("Descripcion")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string DDescripcion { get; set; }
        [DisplayName("Orden")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual short NOrden { get; set; }
        [Required]
        public virtual int NVersionHibernate { get; set; }
        [DisplayName("URL")]        
        public virtual string DUrl { get; set; }
        [DisplayName("Icono")]        
        public virtual string DIcono { get; set; }
        [DisplayName("Ult. Operación")]        
        public virtual DateTime? FUltimaOperacion { get; set; }
        //esto debe ser una relacion 0..1, pero no encontre otra manera de mapearlo que no sea como bag  ya que se mapeo con una tabla intermedia      
        public virtual IList<Actividad> Actividades { get; set; }                
      
        [DisplayName("Actividad")]
        //se usa para el formulario y despues en la accion de editar se asocia a la lista
        public virtual Actividad ActividadAsociada { get; set; }
        //{
        //    get
        //    {
        //        if (Actividades != null && Actividades.Count > 0)
        //        {
        //            this.actividadAsociada = this.Actividades[0];
        //            return this.actividadAsociada;
        //        }
        //        return null;
        //    }  
        //    set
        //    {
        //        actividadAsociada = value;
        //        this.Actividades.Add(value);
        //    }
        //}
    }
}
