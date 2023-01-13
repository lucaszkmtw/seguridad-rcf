using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class AcreedorMap: SubclassMap<Acreedor>
    {

        public AcreedorMap()
        {
            Table("SEG_ACREEDOR");
            KeyColumn("C_ID");
            Map(x => x.NumeroCuit, "D_CUIT");
            Map(x => x.RazonSocial, "D_RAZON_SOCIAL");
        }
        
    }
    public class Acreedor : Usuario
    {
        public Acreedor() { }

        /// <summary>
        /// Nro de cuit dela acreedor
        /// </summary>
        public virtual string NumeroCuit { get; set; }
        /// <summary>
        /// Razon social del acreedor
        /// </summary>
        public virtual string RazonSocial { get; set; }

        public override string GetDescripcionUsuario()
        {
            return this.NumeroCuit + " - " + this.RazonSocial;
        }
    }
}
