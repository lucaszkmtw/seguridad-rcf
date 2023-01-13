using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class NovedadRolNodoMap : ClassMap<NovedadRolNodo>
    {
        public NovedadRolNodoMap()
        {
            Table("SEG_NOVEDAD_ROL");
            Id(x => x.Id, "C_ID").GeneratedBy.SequenceIdentity("SEG_NOVEDAD_ROL_SQ");
            References(x => x.Rol, "C_ID_ROL").Class(typeof(Rol)).Cascade.None();
            References(x => x.NodoFuncional, "C_ID_NODO_FUNCIONAL").Class(typeof(NodoFuncional)).Cascade.None();
            References(x => x.Novedad, "C_ID_NOVEDAD").Class(typeof(Novedad)).Cascade.None();
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
    }

    public class NovedadRolNodo : BaseEntity
    {
        #region // Propiedades Publicas
        public virtual Rol Rol { get; set; }
        public virtual NodoFuncional NodoFuncional{ get; set; }
        public virtual Novedad Novedad { get; set; }
        #endregion

    }
}
