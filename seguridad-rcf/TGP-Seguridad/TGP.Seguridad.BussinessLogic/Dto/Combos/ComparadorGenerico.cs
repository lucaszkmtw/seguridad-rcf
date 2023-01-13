using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGP.Seguridad.BussinessLogic.Dto
{
    public class ComparadorGenerico
    {
        public static string ACTIVAROPCIONROL = "#Comparar-Rol";
        public static string ACTIVAROPCIONMENUES = "#Comparar-MenuOpcion";
        public static string ACTIVAROPCIONACTIVIDADES = "#Comparar-Actividad";
        public static string ACTIVAROPCIONNODOS = "#Comparar-NodoFuncional";
        public static string ACTIVAROPCIONESTRUCTURAS = "#Comparar-EstructuraFuncional";

        public const string MENUOPCION = "MENUOPCION";
        public const string ACTIVIDAD = "ACTIVIDAD";
        public const string ROL = "ROL";
        public const string NODO = "NODO";
        public const string ESTRUCTURA = "ESTRUCTURA";

        public ComparadorGenerico()
        {
            ElementosOrigen = new List<ElementoComparar>();
            ElementosDestino = new List<ElementoComparar>();
        }

        public string TipoComparador { get; set; }
        public string EstructuraSeleccionada { get; set; } = null;
        public string OpcionActivar { get; set; }
        public IList<ElementoComparar> ElementosOrigen { get; set; }
        public IList<ElementoComparar> ElementosDestino { get; set; }
        public string TextoCopiarDestino { get; set; }
        public string TextoCopiarDestinoMasivo { get; set; }
        public string TextoEliminarDestino { get; set; }
    }

    public class ElementoComparar
    {
        public long Id { get; set; }
        public char Estado { get; set; } = 'M';
        public int Version { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Padre { get; set; } = null;
        public string CodigoEstructura { get; set; } = null;
        public string DescripcionEstructura { get; set; } = null;
    }
}
