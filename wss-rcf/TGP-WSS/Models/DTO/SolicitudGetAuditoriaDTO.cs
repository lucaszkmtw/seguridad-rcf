using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Models.DTO
{
    public class SolicitudGetAuditoria
    {
        public string EstructuraFuncional { get; set; }
        public string Usuario { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string Actividad { get; set; }
        public string Observacion { get; set; }
    }
}