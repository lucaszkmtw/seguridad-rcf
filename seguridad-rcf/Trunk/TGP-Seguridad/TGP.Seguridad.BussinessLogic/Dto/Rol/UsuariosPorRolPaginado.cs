using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class UsuariosPorRolPaginado
    {
        public long IdUsuario { get; set; }
        public string Estructura { get; set; }
        public string Rol { get; set; }
        public string Nodo { get; set; }
        public string NombreUsuario { get; set; }
        public long RecordsTotal { get; set; }
    }
}
