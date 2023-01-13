using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class RolUsuarioViewModel
    {
        public long IdUsuario { get; set; }
        public string Estructura { get; set; }
        public string Rol { get; set; }
        public bool RolEsMultinodo { get; set; }
        public IList<string> Nodos { get; set; }
        public string NombreUsuario { get; set; }
    }
}
