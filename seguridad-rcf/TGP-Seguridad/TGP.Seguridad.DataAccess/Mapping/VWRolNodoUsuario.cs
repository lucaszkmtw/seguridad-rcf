using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class VWRolNodoUsuarioMap : ClassMap<VWRolNodoUsuario>
    {
        public VWRolNodoUsuarioMap()
        {
            Table("VW_ROL_NODO_USUARIO");
            Id(x => x.Id, "C_ID");
            Map(x => x.NombreUsuario, "D_USUARIO");
            Map(x => x.IdUsuario, "ID_USUARIO");
            Map(x => x.RazonSocial, "D_RAZON_SOCIAL");
            Map(x => x.IdNodo, "ID_NODO");
            Map(x => x.DescripcionNodo, "D_DESCRIPCION");
            Map(x => x.CodigoNegocio, "C_NEGOCIO");
            Map(x => x.IdRol, "ID_ROL");
            Map(x => x.DescripcionRol, "DESC_ROL");
            Map(x => x.CodigoRol, "C_CODIGO_ROL");
            Map(x => x.IdEstructuraFuncional, "C_ID_ESTRUCTURA_FUNCIONAL");
            Map(x => x.CodigoEstructuraFuncional, "COD_ESTRUCTURA_FUNCIONAL");
            Map(x => x.DescripcionEstructuraFuncional, "D_ESTRUCTURA_FUNCIONAL");
        }
    }

    public class VWRolNodoUsuario : BaseEntity
    {
        #region // Propiedades Publicas //
        public virtual string NombreUsuario { get; set; }
        public virtual long IdUsuario { get; set; }
        public virtual string RazonSocial { get; set; }

        public virtual long IdNodo { get; set; }
        public virtual string DescripcionNodo { get; set; }
        public virtual string CodigoNegocio { get; set; }

        public virtual long IdRol { get; set; }
        public virtual string CodigoRol { get; set; }
        public virtual string DescripcionRol { get; set; }

        public virtual long IdEstructuraFuncional { get; set; }
        public virtual string CodigoEstructuraFuncional { get; set; }
        public virtual string DescripcionEstructuraFuncional { get; set; }

        #endregion
    }
}
