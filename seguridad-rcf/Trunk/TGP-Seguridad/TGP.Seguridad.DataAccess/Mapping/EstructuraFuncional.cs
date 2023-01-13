using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class EstructuraFuncionalMap : ClassMap<EstructuraFuncional>
    {
        public EstructuraFuncionalMap()
        {
            Table("SEG_ESTRUCTURA_FUNCIONAL");
            Id(x => x.Id, "C_ID").GeneratedBy.Assigned();
            References(x => x.Usuario, "C_ID_USUARIO_LOGIN").Class(typeof(Usuario)).Cascade.None();
            Map(x => x.DescripcionEstructura, "D_DESCRIPCION");
            Map(x => x.Codigo, "C_CODIGO");
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            Map(x => x.SiBloqueado, "M_BLOQUEADO");
            HasMany(x => x.Nodos).KeyColumn("C_ID_ESTRUCTURA_FUNCIONAL").Inverse().Cascade.None();
            HasMany(x => x.Roles).KeyColumn("C_ID_ESTRUCTURA_FUNCIONAL").Inverse().Cascade.None();
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
    }

    public class EstructuraFuncional : BaseEntity
    {
        #region // Propiedades Publicas //
        /// <summary>
        /// Propiedad que modela la columna ID 
        /// </summary>
        /// <summary>
        /// Usuario responsable de dar de alta la estrutura
        /// </summary>
        public virtual Usuario Usuario { get; set; }

        /// <summary>
        /// Descripcion / Nombre de la estructura
        /// </summary>
        public virtual string DescripcionEstructura { get; set; }

        /// <summary>
        /// Codigo de la estrutura por lo general son las iniciales
        /// </summary>
        public virtual string Codigo { get; set; }

        /// <summary>
        /// Fecha de la ultima operacion
        /// </summary>
        public virtual DateTime? FechaUltimaOperacion { get; set; }

        /// <summary>
        /// Marca que determina si la estrutura esta bloquedad
        /// </summary>
        public virtual bool SiBloqueado { get; set; }

        /// <summary>
        /// Nodos funcioales asociados al la Estrurtura funcional
        /// </summary>
        public virtual IList<NodoFuncional> Nodos { get; set; }
        
        ///// <summary>
        ///// Roles asociados al la Estrurtura funcional
        ///// </summary>
        public virtual IList<Rol> Roles { get; set; }
        #endregion

    }
}
