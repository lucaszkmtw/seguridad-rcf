using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class EstructuraFuncionalDTO
    {
        public long Id { get; set; }
        public int Version { get; set; }

        /// <summary>
        /// Descripcion / Nombre de la estructura
        /// </summary>
        [DisplayName("Descripción")]
        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(160, MinimumLength = 2, ErrorMessage = "El campo debe poseer entre 2 y 160 cáracteres.")]
        public string DescripcionEstructura { get; set; }

        /// <summary>
        /// Codigo de la estrutura por lo general son las iniciales
        /// </summary>
        [DisplayName("Código")]
        [Required(ErrorMessage = "Campo requerido.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El campo debe poseer entre 2 y 50 cáracteres.")]
        public string Codigo { get; set; }

        /// <summary>
        /// Fecha de la ultima operacion
        /// </summary>
        [DisplayName("Últ. Operación")]
        public DateTime? FechaUltimaOperacion { get; set; }

        /// <summary>
        /// Marca que determina si la estrutura esta bloquedad
        /// </summary>
        [DisplayName("Si Bloqueado?")]
        public bool SiBloqueado { get; set; }
    }
}
