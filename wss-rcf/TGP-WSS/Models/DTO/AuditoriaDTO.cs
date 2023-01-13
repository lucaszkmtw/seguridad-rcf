using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Models.DTO
{
    public class AuditoriaDTO
    {

        public string Usuario { get; set; }

        public DateTime FechaAccion { get; set; }

        public string EstructuraFuncional { get; set; }

        public string Actividad { get; set; }

        public string Observacion { get; set; }

    }
}