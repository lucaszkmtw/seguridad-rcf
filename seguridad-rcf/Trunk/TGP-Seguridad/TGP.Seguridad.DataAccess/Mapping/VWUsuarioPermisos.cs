using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGP.Seguridad.DataAccess.Generics;
using TGPCommonAnnotationsLibrary.Common;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class VWUsuarioPermisosMap : ClassMap<VWUsuarioPermisos>
    {
        public VWUsuarioPermisosMap()
        {

            Table("VW_USUARIO_PERMISOS");
            CompositeId()
                .KeyProperty(x => x.IdEstructuraFuncional, "C_ID_ESTRUCTURA_FUNCIONAL")
                .KeyProperty(x => x.DescripcionNodoFuncional, "NODOFUNCIONAL")
                .KeyProperty(x => x.DesripcionActividad, "ACTIVIDAD")
                .KeyProperty(x => x.Usuario, "D_USUARIO");
            Map(x => x.CodigoEstructura, "C_CODIGO_ESTFUNC");
            References(x => x.Rol, "C_ID_ROL").Class(typeof(Rol)).Cascade.None();
            //Version(x => x.Version).Column("N_VERSION_HIBERNATE").UnsavedValue("0").Access.Property();
        }

    }

    public class VWUsuarioPermisos : BaseEntity
    {

        public VWUsuarioPermisos() { }
        [IsId]
        public virtual long IdEstructuraFuncional { get; set; }
        [IsId]
        public virtual string DescripcionNodoFuncional { get; set; }
        [IsId]
        public virtual string DesripcionActividad { get; set; }
        [IsId]
        public virtual string Usuario { get; set; }

        public virtual string CodigoEstructura { get; set; }
        public virtual Rol Rol { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as VWUsuarioPermisos;
            if (t == null) return false;
            if (IdEstructuraFuncional == t.IdEstructuraFuncional
             && DescripcionNodoFuncional == t.DescripcionNodoFuncional
             && DesripcionActividad == t.DesripcionActividad
             && Usuario == t.Usuario)
                return true;

            return false;
        }
        public override int GetHashCode()
        {
            int hash = GetType().GetHashCode();
            hash = (hash * 397) ^ IdEstructuraFuncional.GetHashCode();
            hash = (hash * 397) ^ DescripcionNodoFuncional.GetHashCode();
            hash = (hash * 397) ^ DesripcionActividad.GetHashCode();
            hash = (hash * 397) ^ Usuario.GetHashCode();

            return hash;
        }
    }
    
}
