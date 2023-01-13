using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.API.Models.DTO
{
    public class ResultLoginMobileDTO
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string UsuarioID { get; set; }
        public string Identificacion { get; set; }
        public string Descripcion { get; set; }
        public string SiValido { get; set; }
        public List<string> ActividadesPermitidas { get; set; }
        public string TokenSession { get; set; }
        public string FechaExpiracionTokenSession { get; set; }
        public Byte[] Avatar { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        // Propiedades del Nominado 
        public long Documento { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }

        // Propiedades del Acreedor
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }

        public virtual bool EsAcreedor
        {
            get; set;
        }

    }
}