using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class ActividadDTO
    {

        public long Id { get; set; }
        public int Version { get; set; }

        /// <summary>
        /// Estructura Funcional 
        /// </summary>
        [DisplayName("Estructura Funcional")]
        [Required(ErrorMessage = "Campo requerido.")]
        public string EstructuraFuncional { get; set; }

        /// <summary>
        /// Codigo de la actividad
        /// </summary>
        [DisplayName("Codigo")]
        [Required(ErrorMessage = "Campo requerido.")]
        public string Codigo { get; set; }

        /// <summary>
        /// Descripcion de la actividad
        /// </summary>
        [DisplayName("Descripción")]
        [Required(ErrorMessage = "Campo requerido.")]
        public string Descripcion { get; set; }
    }
}
