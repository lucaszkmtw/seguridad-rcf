using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class AsignacionMasivaViewModel
    {
        public AsignacionMasivaViewModel()
        {
            Nodos = new List<string>();
            Usuarios = new List<string>();
        }
  

        public string Estructura { get; set; }
        
        public string Rol { get; set; }

        public IList<string> Nodos { get; set; }

        public IList<string> Usuarios { get; set; }

    }
}
