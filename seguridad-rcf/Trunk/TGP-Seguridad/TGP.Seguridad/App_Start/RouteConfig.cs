using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TGP.Seguridad.App_Start
{
    /// <summary>
    /// Clase que encapsula la funcionalidad relacionada con la redirección de peticiones
    /// </summary>
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //Patron que se incluye para evitar que las solicitudes de 
            //los archivos de recursos Web como WebResource.axd o ScriptResource.axd se 
            //transmitan a un controlador.
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Mapeoo de redireccion
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
