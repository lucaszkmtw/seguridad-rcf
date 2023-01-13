using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;
using TGPCommonAnnotationsLibrary.Common;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class BServicioMap : ClassMap<BServicio>
    {
        public BServicioMap()
        {
            Table("BSERVICIO");
            CompositeId()
                    .KeyProperty(x => x.AA_EJERVG, "AA_EJERVG")
                    .KeyProperty(x => x.C_SERVICIO, "C_SERVICIO");
            Map(x => x.XL_SERVICIO, "XL_SERVICIO");
        }
    }
    public class BServicio : BaseEntity
    {
        [IsId]
        public virtual int AA_EJERVG { get; set; }
        [IsId]
        public virtual int C_SERVICIO { get; set; }
        public virtual string XL_SERVICIO { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as BServicio;
            if (t == null) return false;
            if (AA_EJERVG == t.AA_EJERVG
             && C_SERVICIO == t.C_SERVICIO)
                return true;

            return false;
        }
        public override int GetHashCode()
        {
            int hash = GetType().GetHashCode();
            hash = (hash * 397) ^ AA_EJERVG.GetHashCode();
            hash = (hash * 397) ^ C_SERVICIO.GetHashCode();

            return hash;
        }
    }
}
