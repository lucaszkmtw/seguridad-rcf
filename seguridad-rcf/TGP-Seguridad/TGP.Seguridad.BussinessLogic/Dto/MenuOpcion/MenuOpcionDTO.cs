using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class MenuOpcionDTO
    {
        public long Id { get; set; }

        public int Version { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string MenuOpcionPadre { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string EstructuraFuncional { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string Codigo { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        public virtual short NumeroOrden { get; set; }

        public virtual string Url { get; set; }

        public virtual string Icono { get; set; }
        
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string ActividadAsociada { get; set; }

    }
}
