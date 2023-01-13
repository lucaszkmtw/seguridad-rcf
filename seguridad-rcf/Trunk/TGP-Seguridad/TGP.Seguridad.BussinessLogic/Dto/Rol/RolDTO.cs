using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class RolDTO
    {
        public long Id { get; set; }
        public int Version { get; set; }
        public IList<string> Actividades { get; set; }
        public string UsuarioAlta { get; set; }
        [Required(ErrorMessage = "El código del rol es requerido")]
        public string Codigo { get; set; }
        [Required(ErrorMessage = "La descripción del rol es requerida")]
        public string Descripcion { get; set; }
        public string EstructuraFuncional { get; set; }
        public string TipoNodoFuncional { get; set; }
        public bool SiDelegable { get; set; }
        [Required(ErrorMessage = "La información del rol es requerida")]
        public bool EsMultinodo { get; set; }
        public string InformacionRol { get; set; }

    }
}
