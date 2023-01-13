using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TGP.WSS
{
    
    public abstract class Usuario {

        [ScaffoldColumn(false)]
        public virtual long CId { get; set; }

        [DisplayName("Autenticación")]
        public virtual TipoAutenticacion TipoAutenticacion { get; set; }

        public virtual Usuario UsuarioLogin { get; set; }

        [DisplayName("Tipo")]
        public virtual TipoUsuario TipoUsuario { get; set; }

        [DisplayName("Usuario")]
        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "El Usuario debe poseer entre 4 y 30 Caracteres")]
        public virtual string DUsuario { get; set; }

        [DisplayName("Password")]
        [StringLength(30, MinimumLength = 8, ErrorMessage="El Password debe poseer entre 8 y 30 Caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "No se admiten caracteres especiaes")]
        [Required(ErrorMessage = "Campo requerido.")]        
        public virtual string DPassword { get; set; }

        [DisplayName("Confirmar Password")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "El Password debe poseer entre 8 y 30 Caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "No se admiten caracteres especiaes")]
        [Compare("DPassword", ErrorMessage = "El Password y la confirmación no son iguales.")]
        [Required(ErrorMessage = "Campo requerido.")]
        public virtual string DPassword2 { get; set; }

        [DisplayName("Fecha de Alta")]        
        public virtual DateTime FAlta { get; set; }

        [DisplayName("Fecha de Baja")]        
        public virtual DateTime? FBaja { get; set; }

        [DisplayName("Activo")]        
        public virtual bool MActivo { get; set; }

        [DisplayName("Bloqueado")]
        public virtual bool MBloqueado { get; set; }

        [ScaffoldColumn(false)]
        public virtual int NVersionHibernate { get; set; }

        [DisplayName("E-Mail")]
        [Required(ErrorMessage = "Campo requerido.")]        
        [EmailAddress(ErrorMessage = "Formato de E-Mail invalido")]        
        public virtual string DMail { get; set; }

        [DisplayName("Avatar")]
        public virtual byte[] BAvatar { get; set; }

        public virtual DateTime? FUltimaOperacion { get; set; }

        public virtual string DHashResetClave { get; set; }

        [DisplayName("Tel. Fijo")]
        //no se usa mas --> borrar mas adelante
        public virtual string DTelefono { get; set; }

        [DisplayName("Celular")]
        //no se usa mas --> borrar mas adelante
        public virtual string DCelular { get; set; }
        public virtual DateTime? FUltimoIntento { get; set; }
        [DisplayName("Intentos")]
        public virtual int NCantIntentos { get; set; }
        public virtual string CCodVerifMail { get; set; }
        [DisplayName("Ult. Intento")]
        public virtual DateTime FUltIntento { get; set; }
        public virtual bool MVerificaMail { get; set; }
        [DisplayName("Roles")]
        public virtual IList<RolNodoUsuario> RolesNodoUsuario { get; set; }

        [DisplayName("Tipo")]
        public virtual string TipoTelefono { get; set; }
        [DisplayName("Cod. Area")]
        [Required(ErrorMessage = "El campo cod. area (Telefono) es requerido.")]
        [RegularExpression(@"^([0-9]{2,5})$", ErrorMessage = "Formato invalido para cod. area (Telefono).")]
        public virtual string CodArea { get; set; }
        [DisplayName("Número")]
        [Required(ErrorMessage = "El campo numero (Telefono) es requerido.")]
        [RegularExpression(@"^([0-9]{6,9})$", ErrorMessage = "Formato invalido número (Telefono).")]
        public virtual string NumeroTel { get; set; }
        [DisplayName("Interno")]        
        [RegularExpression(@"^([0-9]{0,5})$", ErrorMessage = "Formato invalido para interno (Telefono).")]
        public virtual string Interno { get; set; }

        public virtual TerminosCondiciones TerminosYCondiciones { get; set; }

        [DisplayName("Fecha de Aceptación")]
        public virtual DateTime FAceptacionTerminos { get; set; }

        //metodo a sobreescribir en las subclases
        public virtual String getDescripcionUsuario() 
        {
            return DUsuario;
        }
        
        public virtual bool isAcreedor()
        {            //Imlementar en las subclases
            return false;
        }

        public virtual bool isNominado()
        {            //Imlementar en las subclases
            return false;
        }

        public virtual bool isInnominado()
        {            //Imlementar en las subclases
            return false;
        }
    }
}
