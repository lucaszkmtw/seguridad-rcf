using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.API.Models.DTO.Mobile
{
    public class RequestSendEmailMobileDTO
    {
        public string Usuario { get; set; }

        public string FromEmail { get; set; }

        public string Asunto { get; set; }

        public string Mensaje { get; set; }
    }
}