using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class RolDelegadoAdminMap : ClassMap<RolDelegadoAdmin>
    {
        public RolDelegadoAdminMap()
        {
            Table("SEG_ROL_DELEGADO_ADMIN");
            Id(x => x.Id, "C_ID").GeneratedBy.Assigned();
            References(x => x.Rol, "C_ID_ROL").Class(typeof(Rol)).Cascade.None();
            References(x => x.AdminLocal, "C_ID_ADMIN_LOCAL").Class(typeof(AdministradorLocal)).Cascade.None();
            References(x => x.Usuario, "C_ID_USUARIO").Class(typeof(Usuario)).Cascade.None();
            References(x => x.UsuarioAlta, "C_ID_USUARIO_LOGIN").Class(typeof(Usuario)).Cascade.None();
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
    }
    public class RolDelegadoAdmin: BaseEntity
    {
        public virtual Rol Rol { get; set; }
        public virtual AdministradorLocal AdminLocal { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario UsuarioAlta { get; set; }
        public virtual DateTime? FechaUltimaOperacion { get; set; }
    }
}
