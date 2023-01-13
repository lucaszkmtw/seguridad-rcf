using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class NodoFuncionalDTO
    {
        public long Id { get; set; }
        public int Version { get; set; }
        [Required(ErrorMessage = "Campo requerido.")]
        public string Codigo { get; set; }
        [Required(ErrorMessage = "Campo requerido.")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Campo requerido.")]
        public string EstructuraFuncional { get; set; }
        public string TipoNodoFuncional { get; set; }
        public string NodoPadre { get; set; }
        public bool SiDescentralizado { get; set; }
        public bool SiMultinodo { get; set; }
    }
}
