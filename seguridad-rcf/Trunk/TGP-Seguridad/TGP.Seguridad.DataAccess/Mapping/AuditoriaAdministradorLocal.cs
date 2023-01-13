using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{

    public class AuditoriaAdministradorLocalMap : ClassMap<AuditoriaAdministradorLocal>
    {
        public AuditoriaAdministradorLocalMap()
        {
            Table("SEG_AUDIT_ADMIN_LOCAL");
            Id(x => x.Id, "C_ID").GeneratedBy.SequenceIdentity("SEG_AUDIT_ADMIN_LOCAL_SQ");
            Map(x => x.CodigoAdminLocal, "C_ID_USUARIO_ADMIN");
            Map(x => x.DescripcionAdminLocal, "D_USUARIO_ADMIN");
            Map(x => x.CodigoRol, "C_ROL");
            Map(x => x.DescripcionRol, "D_ROL");
            Map(x => x.CodigoNodoFuncional, "C_NODO_FUNCIONAL");
            Map(x => x.DescripcionNodoFuncional, "D_NODO_FUNCIONAL");
            Map(x => x.NombreUsuario, "C_USUARIO");
            Map(x => x.FechaOperacion, "F_OPERACION");
            Map(x => x.DescripcionOperacion, "D_OPERACION");
            Map(x => x.UsuarioAlta, "C_USUARIO_LOGIN");
            Map(x => x.DescripcionEstructura, "D_ESTRUCTURA_FUNCIONAL");
            Map(x => x.CodigoEstructura, "C_ESTRUCTURA_FUNCIONAL");

        }
    }

    public class AuditoriaAdministradorLocal : BaseEntity
    {
        public virtual long CodigoAdminLocal { get; set; }
        public virtual string DescripcionAdminLocal { get; set; }
        public virtual long? CodigoRol { get; set; }
        public virtual string DescripcionRol { get; set; }
        public virtual string CodigoNodoFuncional { get; set; }
        public virtual string DescripcionNodoFuncional { get; set; }
        public virtual string NombreUsuario { get; set; }
        public virtual DateTime FechaOperacion { get; set; }
        public virtual string DescripcionOperacion { get; set; }
        public virtual string UsuarioAlta { get; set; }
        public virtual string DescripcionEstructura { get; set; }
        public virtual string CodigoEstructura { get; set; }
    }
}
