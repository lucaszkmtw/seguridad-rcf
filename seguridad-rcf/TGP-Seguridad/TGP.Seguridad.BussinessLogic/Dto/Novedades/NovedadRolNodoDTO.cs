using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.Common.Dto;

namespace TGP.Seguridad.BussinessLogic.Dto.Novedades
{
    public class NovedadRolNodoDTO : BaseDto
    {
        #region // Propiedades //
        public virtual NovedadDTO Novedad { get; set; }

        public virtual RolDTO Rol { get; set; }

        public virtual NodoFuncionalDTO Nodo { get; set; }

        #endregion
    }
}
