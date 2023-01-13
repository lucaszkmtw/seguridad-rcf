using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.Common.Dto;
using TGPCommonAnnotationsLibrary.Common;

namespace TGP.Seguridad.BussinessLogic.Dto.Usuario
{
    public class VWUsuarioPermisosDTO 
    {
        public VWUsuarioPermisosDTO() { }
        [IsId]
        public virtual long IdEstructuraFuncional { get; set; }
        [IsId]
        public virtual string DescripcionNodoFuncional { get; set; }
        [IsId]
        public virtual string DesripcionActividad { get; set; }
        [IsId]
        public virtual string Usuario { get; set; }

        public virtual string CodigoEstructura { get; set; }
        public virtual RolDTO Rol { get; set; }
    }
}
