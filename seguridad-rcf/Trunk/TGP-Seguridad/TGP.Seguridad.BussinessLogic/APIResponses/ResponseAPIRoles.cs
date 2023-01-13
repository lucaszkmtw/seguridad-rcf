using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.APIResponses
{
    public class ResponseAPIRoles : ResponseAPI
    {
        public List<RolResponse> Roles { get; set; }

    }

    public class RolResponse
    {
        public long IdRol { get; set; }
        public string DescripcionRol { get; set; }
        public string CodigoRol { get; set; }

    }

}
