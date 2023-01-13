using FluentNHibernate.Mapping;
using System;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class NovedadGuardadaMap : ClassMap<NovedadGuardada>
    {
        public NovedadGuardadaMap()
        {
            Table("SEG_NOVEDAD_GUARDADA");
            Id(x => x.Id, "C_ID").GeneratedBy.SequenceIdentity("SEG_NOVEDAD_GUARDADA_SQ");
            References(x => x.Usuario, "C_ID_USUARIO").Class(typeof(Usuario)).Cascade.None();
            References(x => x.Novedad, "C_ID_NOVEDAD").Class(typeof(Novedad)).Cascade.None();
            Map(x => x.FechaAlta, "FH_ALTA");
            Map(x => x.Comentario, "D_COMENTARIO");
            Map(x => x.FechaBaja, "FH_BAJA");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
    }

    public class NovedadGuardada : BaseEntity
    {
        #region // Propiedades Publicas
        public virtual DateTime FechaAlta{ get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Novedad Novedad { get; set; }

        public virtual DateTime? FechaActualizacion { get; set; }
        public virtual DateTime? FechaBaja { get; set; }
        public virtual string Comentario { get; set; }
        #endregion

    }
}
