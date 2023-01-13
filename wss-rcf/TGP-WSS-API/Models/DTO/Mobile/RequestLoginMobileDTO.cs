using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.API.Models.DTO
{
    public class RequestLoginMobileDTO
    {
        /// <summary>
        /// Nombre de usuario a autenticar
        /// </summary>
        public string NombreUsuario { get; set; }
        /// <summary>
        /// Contraseña del usuario a autenticar
        /// </summary>
        public string Contrasena { get; set; }
        /// <summary>
        /// Codigo de la estructura funcional a validar
        /// </summary>
        /// 
        public string CodigoEstructuraFuncional { get; set; }


    }
}