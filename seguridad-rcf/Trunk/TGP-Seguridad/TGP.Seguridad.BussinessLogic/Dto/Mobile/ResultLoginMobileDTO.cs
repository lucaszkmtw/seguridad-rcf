using System;
using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.API.Models.DTO
{
    public class ResultLoginMobileDTO
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string UsuarioID { get; set; }
        public string Descripcion { get; set; }
        public string SiValido { get; set; }
        public string TokenSession { get; set; }
        public string FechaExpiracionTokenSession { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }
        // Propiedades del Acreedor
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }

        public virtual bool EsAcreedor
        {
            get; set;
        }

    }
}