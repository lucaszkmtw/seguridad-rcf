using FluentNHibernate.Mapping;
using System;
using TGP.Seguridad.DataAccess.Generics;
using TGPCommonAnnotationsLibrary.Common;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class TokenMobileMap : ClassMap<TokenMobile>
    {
        public TokenMobileMap()
        {

            Table("SEG_TOKEN_SESION_MOBILE");
            Id(x => x.Id, "C_ID").GeneratedBy.SequenceIdentity("SEG_TOKEN_SESION_MOBILE_SQ"); ;
            Map(x => x.IdUsuario, "C_ID_USUARIO");
            Map(x => x.Token, "X_TOKEN");
            Map(x => x.FechaExpiracion, "F_EXPIRACION");
            Map(x => x.FechaAlta, "F_ALTA");
            Map(x => x.FechaBaja, "F_BAJA");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }

    }

    public class TokenMobile : BaseEntity
    {

        public TokenMobile() { }

        public virtual long IdUsuario { get; set; }
        public virtual string Token { get; set; }
        public virtual DateTime FechaExpiracion { get; set; }
        public virtual DateTime FechaAlta { get; set; }
        public virtual DateTime? FechaBaja { get; set; }

    }
}
