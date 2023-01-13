using FluentNHibernate.Mapping;
using System;
using TGP.Seguridad.DataAccess.Generics;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class ActividadMap: ClassMap<Actividad>
    {

        public ActividadMap()
        {
            Table("SEG_ACTIVIDAD");
            Id(x => x.Id, "C_ID").GeneratedBy.Assigned();
            References(x => x.EstructuraFuncional, "C_ID_ESTRUCTURA_FUNCIONAL").Class(typeof(EstructuraFuncional)).Cascade.None();
            References(x => x.UsuarioResponsable, "C_ID_USUARIO_LOGIN").Class(typeof(Usuario)).Cascade.None();
            Map(x => x.Codigo,"C_CODIGO");
            Map(x => x.Descripcion, "D_DESCRIPCION");
            Map(x => x.FechaUltimaOperacion, "F_ULTIMA_OPERACION");
            Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }
        
    }
    public class Actividad : BaseEntity
    {
        public Actividad() { }
        
        /// Estructura Funcional 
        /// </summary>
        public virtual EstructuraFuncional EstructuraFuncional { get; set; }

        /// <summary>
        /// Usuario Respondable del alta de la relacion
        /// </summary>
        public virtual Usuario UsuarioResponsable { get; set; }

        /// <summary>
        /// Codigo de la actividad
        /// </summary>
        public virtual string Codigo { get; set; }
        /// fecha de ultima operacion
        /// </summary>  
        public virtual DateTime? FechaUltimaOperacion { get; set; }
    }
}
