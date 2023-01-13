using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    #region Mapeo
    public class UsuarioMap : ClassMap<Usuario>
    {
        public UsuarioMap()
        {
            Table("SEG_USUARIO");
            Id(x => x.Id, "C_ID").GeneratedBy.SequenceIdentity("SEG_USUARIO_SQ");
            References(x => x.UsuarioAlta, "C_ID_USUARIO_LOGIN").Class(typeof(Usuario)).Cascade.None();
            References(x => x.TipoAutenticacion, "C_ID_TIPO_AUTENTICACION").Class(typeof(TipoAutenticacion)).Cascade.None();
            References(x => x.TipoUsuario, "C_ID_TIPO_USUARIO").Class(typeof(TipoUsuario)).Cascade.None();
            Map(x => x.NombreUsuario, "D_USUARIO").Not.Nullable();
            Map(x => x.Contrasena, "D_PASSWORD");
            Map(x => x.FechaAlta, "F_ALTA").Not.Nullable();
            Map(x => x.FechaBaja, "F_BAJA");
            Map(x => x.SiActivo, "M_ACTIVO").Not.Nullable();
            Map(x => x.SiBloqueado, "M_BLOQUEADO").Not.Nullable();
            Map(x => x.SiDGA, "M_DGA").Not.Nullable();
            Map(x => x.EMail, "D_MAIL");
            Map(x => x.Avatar, "B_AVATAR");
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            Map(x => x.HashClaveReseteada, "D_HASH_RESET_CLAVE");
            Map(x => x.TelefonoFijo, "D_TELEFONO");
            Map(x => x.TelefonoCelular, "D_CELULAR");
            Map(x => x.CantidadIntentos, "N_CANTIDAD_INTENTOS");
            Map(x => x.FechaUltimoIntento, "F_ULTIMO_INTENTO_LOGIN");
            Map(x => x.TipoTelefono, "C_TIPO_TELEFONO");
            Map(x => x.CodigoAreaTelefono, "N_COD_AREA");
            Map(x => x.NumeroTelefono, "N_NUMERO_TELEFONO");
            Map(x => x.NumeroInterno, "N_INTERNO");
            Map(x => x.CodigoMsaf, "C_MSAF");
            Map(x => x.Descripcion).Formula(@"(select n.d_apellido || ', ' || n.d_nombre from seg_nominado n where n.c_id = C_ID)");
            Map(x => x.EsContadorDelegado).Formula(@"(select count(*) from SEG_ROL_NODO_USUARIO t inner join seg_rol r on r.c_id = t.c_id_rol where t.c_id_usuario = c_id and r.c_codigo= 'AUTORIZ-CON-DELE')");
            HasMany(x => x.RolesNodoUsuario).KeyColumn("C_ID_USUARIO").Inverse().Cascade.AllDeleteOrphan();
            HasMany(x => x.AdministradoresLocales).KeyColumn("C_ID_USUARIO_ADMIN").Inverse().Cascade.AllDeleteOrphan();
            HasMany(x => x.LogAuditoriaConexion).KeyColumn("C_ID_USUARIO").Inverse().Cascade.AllDeleteOrphan();
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();

        }
    }
    #endregion

    #region Entity
    public class Usuario : BaseEntity
    {
        /// <summary>
        /// Usuario responsable de dar de alta la estrutura
        /// </summary>
        public virtual Usuario UsuarioAlta { get; set; }

        ///// <summary>
        ///// Tipo de autenticacion
        ///// </summary>
        public virtual TipoAutenticacion TipoAutenticacion { get; set; }

        ///// <summary>
        ///// Tipo de usuario
        ///// </summary>
        public virtual TipoUsuario TipoUsuario { get; set; }

        /// <summary>
        /// Nombre de usuarios
        /// </summary>
        public virtual string NombreUsuario { get; set; }

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        public virtual string Contrasena { get; set; }

        ///// <summary>
        ///// Confirmacion de la contraseña
        ///// </summary>
        //public virtual string ContrasenaRepetida { get; set; }

        /// <summary>
        /// Fecha de alta del usuario
        /// </summary>
        public virtual DateTime FechaAlta { get; set; }

        /// <summary>
        /// Fecha de baja del usuario
        /// </summary>
        public virtual DateTime? FechaBaja { get; set; }

        /// <summary>
        /// Marca que determina si esta activo o no el usuario
        /// </summary>
        public virtual bool SiActivo { get; set; }

        /// <summary>
        /// Marca que determina si el usuario esta bloqueado o no
        /// </summary>
        public virtual bool SiBloqueado { get; set; }

        /// <summary>
        /// Marca que determina si el usuario es DGA o no
        /// </summary>
        public virtual bool SiDGA { get; set; }

        /// <summary>
        /// Email del usuario
        /// </summary>
        public virtual string EMail { get; set; }

        /// <summary>
        /// Imagen del usuario en la aplicacion
        /// </summary>
        public virtual byte[] Avatar { get; set; }

        /// <summary>
        /// Fecha de ultima operacion del usuario
        /// </summary>
        public virtual DateTime? FechaUltimaOperacion { get; set; }

        /// <summary>
        /// Clave encriptada del usuario
        /// </summary>
        public virtual string HashClaveReseteada { get; set; }

        /// <summary>
        /// Telefono fijo
        /// </summary>
        public virtual string TelefonoFijo { get; set; }

        /// <summary>
        /// Telefono celular
        /// </summary>
        public virtual string TelefonoCelular { get; set; }

        /// <summary>
        /// Cantidad de intentos de ingresos al login
        /// </summary>
        public virtual int? CantidadIntentos { get; set; }

        /// <summary>
        /// Fecha del ultimo intento de ingreso
        /// </summary>
        public virtual DateTime? FechaUltimoIntento { get; set; }

        /// <summary>
        /// Roles del Usuario
        /// </summary>
        public virtual IList<RolNodoUsuario> RolesNodoUsuario { get; set; }

        /// <summary>
        /// Administradores Locales del Usuario
        /// </summary>
        public virtual IList<AdministradorLocal> AdministradoresLocales { get; set; }

        /// <summary>
        /// Tipo de telefono
        /// </summary>
        public virtual string TipoTelefono { get; set; }

        /// <summary>
        /// Codigo de area del telefono
        /// </summary>
        public virtual string CodigoAreaTelefono { get; set; }

        /// <summary>
        /// Numero de Telefono
        /// </summary>
        public virtual string NumeroTelefono { get; set; }

        /// <summary>
        /// Numero de interno 
        /// </summary>
        //[StringValidator(MaxLength = 5)]
        public virtual string NumeroInterno { get; set; }

        public virtual int? CodigoMsaf { get; set; }

        public virtual IList<AuditoriaConexion> LogAuditoriaConexion { get; set; }

        public virtual string GetDescripcionUsuario()
        {
            return NombreUsuario;
        }

        public virtual bool EsContadorDelegado { get; set; }
    }
    #endregion
}
