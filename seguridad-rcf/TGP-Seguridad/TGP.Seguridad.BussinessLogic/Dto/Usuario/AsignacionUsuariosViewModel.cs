using System.Collections.Generic;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class AsignacioUsuariosViewModel
    {
        public string DescripcionEstructuraFuncional { get; set; }
        public string DescripcionNodo { get; set; }
        public string NombreUsuario { get; set; }
        public string RazonSocial { get; set; }
        public long RecordsTotal { get; set; }

        public IList<string> ListaDescripcionNodo { get; set; } = null;
        public string DescripcionRol { get; set; }
    }
}
