using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class NodoFuncionalMap : ClassMap<NodoFuncional>
    {
        public NodoFuncionalMap()
        {
            Table("SEG_NODO_FUNCIONAL");
            Id(x => x.Id, "C_ID").GeneratedBy.Assigned();
            References(x => x.Usuario, "C_ID_USUARIO_LOGIN").Class(typeof(Usuario)).Cascade.None();
            References(x => x.NodoFuncionalPadre, "C_ID_PADRE").Class(typeof(NodoFuncional)).Cascade.SaveUpdate();
            References(x => x.EstructuraFuncional, "C_ID_ESTRUCTURA_FUNCIONAL").Class(typeof(EstructuraFuncional));
            References(x => x.TipoNodoFuncional, "C_ID_TIPO_NODO").Class(typeof(TipoNodoFuncional));
            Map(x => x.Descripcion, "D_DESCRIPCION");
            Map(x => x.Codigo, "C_NEGOCIO");
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            Map(x => x.SiDescentralizado, "M_DESCENTRALIZADO");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
    }

    public class NodoFuncional : BaseEntity
    {
        /// <summary>
        /// Estructura funcional
        /// </summary>
        public virtual EstructuraFuncional EstructuraFuncional { get; set; }
        /// <summary>
        ///Nodo funcional padre 
        /// </summary>
        public virtual NodoFuncional NodoFuncionalPadre { get; set; }
        /// <summary>
        /// usuaro responsable de guardar la relacion
        /// </summary>
        public virtual Usuario Usuario { get; set; }

        /// <summary>
        /// Numero de Codigo de negocion (JURI)
        /// </summary>
        public virtual string Codigo { get; set; }
        /// <summary>       
        /// <summary>
        /// Marca que determina si el Organismo es descentralizado o no
        /// </summary>
        public virtual bool SiDescentralizado { get; set; }
        /// <summary>
        /// Fecha de ultima operacion
        /// </summary>
        public virtual DateTime? FechaUltimaOperacion { get; set; }
        public virtual TipoNodoFuncional TipoNodoFuncional { get; set; }
    }
}
