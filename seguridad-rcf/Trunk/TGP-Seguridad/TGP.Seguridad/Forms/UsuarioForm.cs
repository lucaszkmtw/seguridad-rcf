using System.ComponentModel.DataAnnotations;

namespace TGP.Seguridad.Forms
{
    /// <summary>
    /// Clase utilizada para modelar los datos del logon
    /// </summary>
    public class UsuarioForm
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required]
        [Display(Name = "Usuario")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "No se admiten caracteres especiales")]
        public string usuario { get; set; }

        /// <summary>
        /// Contraseña ingresada por el usuario
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string clave { get; set; }
    }
}