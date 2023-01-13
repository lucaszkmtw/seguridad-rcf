using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{

    public class Novedad
    {
        public Novedad() {
            this.Roles = new List<Rol>();
        }
        public virtual long CId { get; set; }
       
        public virtual int NVersionHibernate { get; set; }

        [DisplayName("Descripción")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string DNovedad { get; set; }

        [DisplayName("Fecha Desde")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual DateTime FDesde { get; set; }

        [DisplayName("Fecha Hasta")]                                             
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public virtual DateTime? FHasta { get; set; }
        
        public virtual IList<Rol> Roles { get; set; }

        public virtual EstructuraFuncional getEstructura() {
            List<Rol> roles = new List<Rol>();
            roles.AddRange(Roles);
            if (roles != null && roles.Count > 0) {
                return roles[0].EstructuraFuncional;
            }
            return null;
        }
    }
}
