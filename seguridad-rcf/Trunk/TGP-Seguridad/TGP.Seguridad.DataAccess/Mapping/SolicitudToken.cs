using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class SolicitudTokenMap : ClassMap<SolicitudToken>
    {
        public SolicitudTokenMap()
        {
            Table("TGP_PARAMETRO");
            Id(x => x.CId, "C_ID").GeneratedBy.Assigned();
            Map(x => x.Nombre, "D_NOMBRE");
            Map(x => x.Valor, "D_VALOR");
            Map(x => x.Descripcion, "D_DESCRIPCION");
            Map(x => x.TipoDato, "D_TIPO_DATO");
            Map(x => x.FormatoTransf, "D_FORMATO_TRANSF");
            Map(x => x.Usuario, "C_USUARIO");
            Map(x => x.FechaAlta, "FH_ALTA");
            Map(x => x.FechaModificacion, "FHU_ACTUALIZ");
        }
    }

    public class SolicitudToken : BaseEntity
    {
        public SolicitudToken() { }
        public virtual long CId { get; set; }
        public virtual string Valor { get; set; }
        public virtual string TipoDato { get; set; }
        public virtual string FormatoTransf { get; set; }
        public virtual string Usuario { get; set; }
        public virtual DateTime FechaAlta { get; set; }
        public virtual DateTime FechaModificacion { get; set; }
    }
}
