

using System.ComponentModel.DataAnnotations;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class CambioPasswordViewModel : UsuarioDTO
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "No se admiten caracteres especiales")]
        [MinLength(8, ErrorMessage = "Debe poseer al menos 8 caracteres.")]
        public string NuevaContraseña { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("NuevaContraseña", ErrorMessage = "La contraseña y la confirmación no son iguales.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "No se admiten caracteres especiales")]
        [MinLength(8, ErrorMessage = "Debe poseer al menos 8 caracteres.")]
        public string NuevaContraseñaConfirmacion { get; set; }
        
    }
}
