using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class RolNodoUsuarioMap : ClassMap<RolNodoUsuario>
    {
        public RolNodoUsuarioMap()
        {
            Table("SEG_ROL_NODO_USUARIO");
            Id(x => x.Id, "C_ID").GeneratedBy.SequenceIdentity("SEG_ROL_NODO_USUARIO_SQ");
            References(x => x.Rol, "C_ID_ROL").Class(typeof(Rol)).Cascade.None();
            References(x => x.NodoFuncional, "C_ID_NODO_FUNCIONAL").Class(typeof(NodoFuncional)).Cascade.None();
            References(x => x.Usuario, "C_ID_USUARIO").Class(typeof(Usuario)).Cascade.None();
            References(x => x.UsuarioAlta, "C_ID_USUARIO_LOGIN").Class(typeof(Usuario)).Cascade.None();
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
    }

    public class RolNodoUsuario: BaseEntity
    {
        #region // Propiedades Publicas //
        /// <summary>
        /// Propiedad que modela la columna ID 
        /// </summary>
        /// <summary>
        /// Rol
        /// </summary>
        public virtual Rol Rol { get; set; }
        /// <summary>
        /// Nodo Funcional
        /// </summary>
        public virtual NodoFuncional NodoFuncional { get; set; }
        /// <summary>
        /// Usuario
        /// </summary>
        public virtual Usuario Usuario { get; set; }
        /// <summary>
        /// Usuario que registro la relacion
        /// </summary>
        public virtual Usuario UsuarioAlta { get; set; }
        /// <summary>
        /// Numero de version
        /// </summary>
        /// <summary>
        /// Fecha de ultima operacion
        /// </summary>
        public virtual DateTime? FechaUltimaOperacion { get; set; }

        #endregion
    }
}
