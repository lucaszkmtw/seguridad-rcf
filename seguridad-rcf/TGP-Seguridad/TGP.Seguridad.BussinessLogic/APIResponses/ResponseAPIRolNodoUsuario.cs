using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.APIResponses
{
    public class ResponseAPIRolNodoUsuario : ResponseAPI
    {
        public RolNodoUsuarioResponse RolesNodoUsuario { get; set; }
    }

    public class RolNodoUsuarioResponse
    {
        public RolResponse Rol { get; set; }
        public List<NodoResponse> NodosFuncionales { get; set; }
    }

}
