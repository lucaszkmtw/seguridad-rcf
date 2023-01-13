using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class TipoAutenticacionMap : ClassMap<TipoAutenticacion>
    {
        public TipoAutenticacionMap()
        {

            Table("SEG_TIPO_AUTENTICACION");
            Id(x => x.Id, "C_ID");
            Map(x => x.Codigo, "C_CODIGO").Not.Nullable();
            Map(x => x.Descripcion, "D_DESCRIPCION");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }

    }

    public class TipoAutenticacion : BaseEntity
    {

        public TipoAutenticacion() { }

        /// <summary>
        /// Codigo de identificacion
        /// </summary>
        public virtual string Codigo { get; set; }

    }
}
