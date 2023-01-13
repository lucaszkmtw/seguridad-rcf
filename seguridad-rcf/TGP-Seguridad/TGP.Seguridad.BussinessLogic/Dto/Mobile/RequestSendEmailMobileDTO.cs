

namespace TGP.Seguridad.BussinessLogic.API.Models.DTO
{
    public class RequestSendEmailMobileDTO
    {
        public string Usuario { get; set; }

        public string FromEmail { get; set; }

        public string Asunto { get; set; }

        public string Mensaje { get; set; }
    }
}