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
    public class UsuarioSIGAFMap : ClassMap<UsuarioSIGAF>
    {
        public UsuarioSIGAFMap()
        {
            Table("BUSUARIO");
            Id(x => x.CUser, "C_USER");
            Map(x => x.XcUser, "XC_USER").Not.Nullable();
            Map(x => x.NDocumento, "N_DOCUMENTO").Not.Nullable();
            Map(x => x.XlTelefono, "XL_TELEFONO");
            Map(x => x.XlEmail, "XL_EMAIL");
            Map(x => x.FechaBaja, "FH_BAJA");
            Map(x => x.SiBloqueado, "M_BLOQUEO");
            Map(x => x.FechaBloqueo, "FH_BLOQUEO");
        }
    }
    #endregion

    #region Entity
    public class UsuarioSIGAF : BaseEntity
    {
        public virtual string CUser { get; set; }
        public virtual string XcUser { get; set; }
        public virtual int NDocumento { get; set; }
        public virtual string XlTelefono { get; set; }
        public virtual string XlEmail { get; set; }
        public virtual int? MSaf { get; set; }
        public virtual DateTime? FechaBaja { get; set; }
        public virtual bool? SiBloqueado { get; set; }
        public virtual DateTime? FechaBloqueo { get; set; }
    }
    #endregion
}
