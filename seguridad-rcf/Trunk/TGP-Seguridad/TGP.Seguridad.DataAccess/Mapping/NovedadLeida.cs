using FluentNHibernate.Mapping;
using System;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class NovedadLeidaMap : ClassMap<NovedadLeida>
    {
        public NovedadLeidaMap()
        {
            Table("SEG_NOVEDAD_LEIDA");
            Id(x => x.Id, "C_ID").GeneratedBy.SequenceIdentity("SEG_NOVEDAD_LEIDA_SQ");
            References(x => x.Usuario, "C_ID_USUARIO").Class(typeof(Usuario)).Cascade.None();
            References(x => x.Novedad, "C_ID_NOVEDAD").Class(typeof(Novedad)).Cascade.None();
            Map(x => x.FechaLectura, "F_LECTURA");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
    }

    public class NovedadLeida : BaseEntity
    {
        #region // Propiedades Publicas
        public virtual DateTime FechaLectura{ get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Novedad Novedad { get; set; }
        #endregion

    }
}
