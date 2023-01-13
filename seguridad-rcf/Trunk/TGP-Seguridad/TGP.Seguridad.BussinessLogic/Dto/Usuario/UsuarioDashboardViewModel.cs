using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class UsuarioDashboardViewModel : UsuarioDTO
    {
        
        public UsuarioDashboardViewModel()
        {
            this.Estructuras = new List<ComboGenerico>();
        }

        public IList<ComboGenerico> Estructuras { get; set; }
    }
}
