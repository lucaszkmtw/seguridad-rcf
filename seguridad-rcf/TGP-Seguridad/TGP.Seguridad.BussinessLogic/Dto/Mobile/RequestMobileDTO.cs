

namespace TGP.Seguridad.BussinessLogic.API.Models.DTO
{
    public class RequestMobileDTO
    {
        //Token de sesion
        public string Token { get; set; }
        //Id del usuario logueado
        public string UserID { get; set; }

        //Todos los request deberian implementar su propio metodo que verifique si es valido
        public virtual bool SiRequestValido()
        {
            if (string.IsNullOrEmpty(this.Token) || string.IsNullOrEmpty(this.UserID))
                return false;
            return true;
        }


    }
}