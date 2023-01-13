using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class NivelJerarquicoMap: ClassMap<NivelJerarquico>
    {

        public NivelJerarquicoMap()
        {
            Table("SEG_NIVEL_JERARQUICO");
            Id(x => x.Id, "C_ID");
            Map(x => x.Descripcion, "D_DESCRIPCION");
            Map(x => x.Codigo, "C_CODIGO");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
        
    }
    public class NivelJerarquico : BaseEntity
    {
        public NivelJerarquico()
        { }

        public virtual long Codigo { get; set; }
    }
}
