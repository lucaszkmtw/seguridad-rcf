using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class CopiaMasivaPermisosViewModel
    {
        public CopiaMasivaPermisosViewModel()
        {
            //Permisos = new List<PermisosViewModel>();
            //Usuarios = new List<string>();
            //CodigosEstructuras = new List<string>();
            //Estructuras = new List<string>();
        }
        public long Id { get; set; }
        public string TipoUsuario { get; set; }
        public IList<PermisosViewModel> Permisos { get; set; }
        public IList<string> Usuarios { get; set; }
        public IList<string> CodigosEstructuras { get; set; }
        public IList<string> Estructuras { get; set; }
    }
}
