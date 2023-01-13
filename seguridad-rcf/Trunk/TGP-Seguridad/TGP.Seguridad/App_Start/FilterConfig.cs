using ErrorManagerTGP.Filters;
using System.Web;
using System.Web.Mvc;

namespace TGP.Seguridad.App_Start
{

    /// <summary>
    /// Clase que permite protejer el codigo colocado en la aplicacion
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Metodo que permite la registracion global de filtros, en este caso customizados
        /// y determinados en ErrorManagerTGP
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
            filters.Add(new SessionFilter());
        }
    }
}
