using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    #region Mapeo
    public class AdministradorLocalMap : ClassMap<AdministradorLocal>
    {
        public AdministradorLocalMap()
        {
            Table("SEG_ADMINISTRADOR_LOCAL");
            Id(x => x.Id, "C_ID").GeneratedBy.Assigned();
            References(x => x.UsuarioAlta, "C_ID_USUARIO_LOGIN").Class(typeof(Usuario)).Cascade.None();
            References(x => x.UsuarioAdmin, "C_ID_USUARIO_ADMIN").Class(typeof(Usuario)).Cascade.None();
            References(x => x.Usuario, "C_ID_USUARIO").Class(typeof(Usuario)).Cascade.None();
            References(x => x.NodoFuncional, "C_ID_NODO_FUNCIONAL").Class(typeof(NodoFuncional)).Cascade.None();
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
            HasMany(x => x.RolesDelegadosAdmins).KeyColumn("C_ID_ADMIN_LOCAL").Inverse().Cascade.AllDeleteOrphan();

        }
    }
    #endregion

    #region Entity
    public class AdministradorLocal: BaseEntity
    {
        public virtual Usuario UsuarioAdmin { get; set; }
        public virtual NodoFuncional NodoFuncional { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario UsuarioAlta { get; set; }
        public virtual DateTime? FechaUltimaOperacion { get; set; }
        public virtual IList<RolDelegadoAdmin> RolesDelegadosAdmins { get; set; }
    }
    #endregion
}
