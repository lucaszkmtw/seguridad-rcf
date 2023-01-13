using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class NovedadMap : ClassMap<Novedad>
    {
        public NovedadMap()
        {
            Table("SEG_NOVEDAD");
            Id(x => x.Id, "C_ID").GeneratedBy.SequenceIdentity("SEG_NOVEDAD_SQ");
            References(x => x.EstructuraFuncional, "C_ID_ESTRUCTURA_FUNCIONAL").Class(typeof(EstructuraFuncional)).Cascade.None();
            Map(x => x.Titulo, "D_TITULO");
            Map(x => x.Descripcion, "D_NOVEDAD");
            Map(x => x.TipoNovedad, "D_TIPO");
            Map(x => x.SiPublica, "M_PUBLICA");
            Map(x => x.FechaDesde, "F_DESDE");
            Map(x => x.FechaHasta, "F_HASTA");
            Map(x => x.NumeroVersion, "D_VERSION");
            HasMany(x => x.ListaNovedadRolNodo).KeyColumn("C_ID_NOVEDAD").Inverse().Cascade.AllDeleteOrphan();
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
    }

    public class Novedad : BaseEntity
    {
        #region // Propiedades Publicas
        public virtual string Titulo { get; set; }
        public virtual DateTime? FechaDesde { get; set; }
        public virtual DateTime? FechaHasta { get; set; }
        public virtual bool SiPublica { get; set; }
        public virtual string TipoNovedad { get; set; }
        public virtual string NumeroVersion { get; set; }
        public virtual EstructuraFuncional EstructuraFuncional { get; set; }
        public virtual IList<NovedadRolNodo> ListaNovedadRolNodo { get; set; }

        #endregion

    }
}
