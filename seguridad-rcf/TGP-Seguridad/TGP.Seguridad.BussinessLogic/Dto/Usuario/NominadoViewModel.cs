using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class NominadoViewModel : UsuarioDTO
    {
        [Required]
        public long NumeroDni { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string NombreNominado { get; set; }

        public ComboGenerico NivelJerarquico { get; set; }

        
    }
}
