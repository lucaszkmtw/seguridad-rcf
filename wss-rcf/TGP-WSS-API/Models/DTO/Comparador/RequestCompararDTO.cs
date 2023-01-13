using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.API.Models.DTO.Comparador
{
    public class RequestCompararDTO
    {
        /// <summary>
        /// Nombre de usuario a autenticar
        /// </summary>
        public string NombreUsuario { get; set; }
        /// <summary>
        /// Key para autenticar la llamada
        /// </summary>
        public string KeyEncrypt { get; set; }
        /// <summary>
        /// Codigo de la estructura funcional a validar
        /// </summary>
        public string CodigoEstructuraFuncional { get; set; }
    }
}