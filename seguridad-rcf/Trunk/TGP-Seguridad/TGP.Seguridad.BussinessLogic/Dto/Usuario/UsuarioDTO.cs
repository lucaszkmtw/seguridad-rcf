using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class UsuarioDTO
    {

        #region Variables
        public static string INTERNA = "1";
        public static string SIGAF = "2";

        public static string USUARIONOMINADO = "N";
        public static string USUARIOACREEDOR = "A";

        public static string EMPLEADO = "1";
        public static string JEFE = "2";
        public static string DIRECTORLINEA = "3";
        public static string DIRECTORGENERALASESORES = "4";
        public static string AUTORIDADESUPERIORES = "5";

        public long Id { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "El Usuario debe poseer entre 4 y 30 Caracteres")]
        public string NombreUsuario { get; set; }

        public byte[] Avatar { get; set; }

        /// <summary>
        /// Codigo de area del telefono
        /// </summary>
        /// 
        [Required(ErrorMessage = "El campo cod. area (Telefono) es requerido.")]
        [RegularExpression(@"^([0-9]{2,5})$", ErrorMessage = "Formato invalido para cod. area (Telefono).")]
        public virtual string CodigoAreaTelefono { get; set; }
        /// <summary>
        /// Numero de Telefono
        /// </summary>
        /// 
        [Required(ErrorMessage = "El campo numero (Telefono) es requerido.")]
        [RegularExpression(@"^([0-9]{6,9})$", ErrorMessage = "Formato invalido número (Telefono).")]
        public virtual string NumeroTelefono { get; set; }

        /// <summary>
        /// Numero de interno 
        /// </summary>
        /// 
        [RegularExpression(@"^([0-9]{0,5})$", ErrorMessage = "Formato invalido para interno (Telefono).")]
        public virtual string NumeroInterno { get; set; }

        [Required(ErrorMessage = "Campo requerido.")]
        [EmailAddress(ErrorMessage = "Formato de E-Mail invalido")]
        public string EMail { get; set; }

        public string DescripcionTipoUsuario { get; set; }

        public string CodigoTipoAutenticacion { get; set; }

        public string CodigoTipoUsuario { get; set; }

        public int Version { get; set; }

        [StringLength(30, MinimumLength = 8, ErrorMessage = "El Password debe poseer entre 8 y 30 Caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "No se adminten caracteres especiaes")]
        [Required(ErrorMessage = "Campo requerido.")]
        public string Contrasena { get; set; }

        [StringLength(30, MinimumLength = 8, ErrorMessage = "El Password debe poseer entre 8 y 30 Caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "No se adminten caracteres especiaes")]
        [Compare("Contrasena", ErrorMessage = "El Password y la confirmación no son iguales.")]
        [Required(ErrorMessage = "Campo requerido.")]
        public string Contrasena2 { get; set; }

        public ComboGenerico TipoAutenticacion { get; set; }

        public bool SiActivo { get; set; }

        public bool SiDGA { get; set; }

        public bool SiBloqueado { get; set; }

        /// <summary>
        /// Cantidad de intentos de ingresos al login
        /// </summary>
        public int? CantidadIntentos { get; set; }
        /// <summary>
        /// Tipo de telefono
        /// </summary>
        public string TipoTelefono { get; set; }

        public virtual string TelefonoFijo { get; set; }

        /// <summary>
        /// Telefono celular
        /// </summary>
        public virtual string TelefonoCelular { get; set; }

        public virtual int? CodigoMsaf { get; set; }
        #endregion
    }
}
