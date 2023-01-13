using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace TGP.WSS.Models.DTO
{
    /// <summary>
    /// Clase que modela el nodo funcional
    /// </summary>
    public class NodoFuncionalDTO
    {
        /// <summary>
        /// Listado de Usuarios pertenecientes al Nodo Funcional
        /// </summary>
        [DataMember]
        public List<UsuarioDTO> Usuarios { get; set; }

    }
}