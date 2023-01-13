using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.API.Models.DTO.Comparador
{
    public class ElementoComparar
    {
        public long Id { get; set; }
        public char Estado { get; set; } = 'M';
        public int Version { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Padre { get; set; } = null;
        public string CodigoEstructura { get; set; } = null;
        public string DescripcionEstructura { get; set; } = null;
    }
}