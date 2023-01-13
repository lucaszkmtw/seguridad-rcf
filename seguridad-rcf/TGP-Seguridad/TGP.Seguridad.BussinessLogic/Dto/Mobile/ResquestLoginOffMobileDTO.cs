

namespace TGP.Seguridad.BussinessLogic.API.Models.DTO
{
    public class ResquestLoginOffMobileDTO: RequestMobileDTO
    {

        public string TokenSession { get; set; }

        public string UsuarioID { get; set; }
    }
}