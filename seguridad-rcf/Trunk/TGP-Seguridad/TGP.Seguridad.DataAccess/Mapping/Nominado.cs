using FluentNHibernate.Mapping;

namespace TGP.Seguridad.DataAccess.Mapping
{
    public class NominadoMap : SubclassMap<Nominado>
    {

        public NominadoMap()
        {

            Table("SEG_NOMINADO");
            KeyColumn("C_ID");
            References(x => x.NivelJerarquico, "C_ID_NIVEL_JERARQUICO").Class(typeof(NivelJerarquico)).Cascade.None();
            Map(x => x.NumeroDni, "N_DNI");
            Map(x => x.Apellido, "D_APELLIDO");
            Map(x => x.NombreNominado, "D_NOMBRE");

        }

    }
    public class Nominado : Usuario
    {
        public Nominado() { }

        public virtual long NumeroDni { get; set; }

        public virtual string Apellido { get; set; }
        public virtual string NombreNominado { get; set; }
        public virtual NivelJerarquico NivelJerarquico { get; set; }

        public override string GetDescripcionUsuario()
        {
            return this.NombreNominado + " " + this.Apellido;
        }
        //public virtual string Dni => NumeroDni.ToString();
    }
}
