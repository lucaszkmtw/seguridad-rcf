using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class TipoNodoFuncionalMap : ClassMap<TipoNodoFuncional>
    {
        public TipoNodoFuncionalMap()
        {
            Table("SEG_TIPO_NODO_FUNCIONAL");
            Id(x => x.Id, "C_ID");
            Map(x => x.Codigo, "C_TIPO_NODO").Not.Nullable();
            Map(x => x.Descripcion, "D_TIPO_NODO");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }

    }
    public class TipoNodoFuncional : BaseEntity
    {
        public virtual string Codigo { get; set; }
    }
}
