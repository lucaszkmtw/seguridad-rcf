using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TGP.Seguridad.BussinessLogic.Config;

namespace TGP.API.SEG
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Propiedad que se utiliza para el logue de accion de la aplicación.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(WebApiApplication));
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MapsterConfig.Config();
            this.ConfigureLog4Net();
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        /// <summary>
        ///  Metodo en el que se levanta y configura log4net
        /// </summary>
        private void ConfigureLog4Net()
        {
            // Configuracion para el log4net -----------------------------------------------------------------
            if (ConfigurationManager.AppSettings["URL_LOG4NET_CONFIG"] == null)
            {
                throw new Exception("No se configuro la key 'URL_LOG4NET_CONFIG' en el Web.config");
            }

            string pathName = System.Web.Hosting.HostingEnvironment.MapPath("~/logs/");
            string projectName = BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName().Name;

            string logErrorType = "_ERROR";
            string logDebugType = "_DEBUG";

            string fileNameSufix = ConfigurationManager.AppSettings["ENVIRONMENT_LOG"];
            string extension = ".log";

            string LogErrorName = pathName + projectName + logErrorType + fileNameSufix + extension;
            string LogDebugName = pathName + projectName + logDebugType + fileNameSufix + extension;

            GlobalContext.Properties["LogErrorName"] = LogErrorName;
            GlobalContext.Properties["LogDebugName"] = LogDebugName;

            string log4netConfigPath = Server.MapPath(ConfigurationManager.AppSettings["URL_LOG4NET_CONFIG"]);
            XmlConfigurator.ConfigureAndWatch(new FileInfo(log4netConfigPath));
            // Configuracion para el log4net -----------------------------------------------------------------
        }
    }
}
