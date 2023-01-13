using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class AcreedorViewModel : UsuarioDTO
    {
        /// <summary>
        /// Nro de cuit dela acreedor
        /// </summary>
        [Required(ErrorMessage = "Numero Cuit es un campo requerido.")]
        public virtual string NumeroCuit { get; set; }
        /// <summary>
        /// Razon social del acreedor
        /// </summary>
        [Required(ErrorMessage = "Razon Social es un campo requerido.")]
        public virtual string RazonSocial { get; set; }
    }
}
