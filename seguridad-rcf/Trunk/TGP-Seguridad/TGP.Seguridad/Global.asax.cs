using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using log4net.Config;
using System.IO;
using System.Web.Compilation;
using log4net;
using TGP.Seguridad.App_Start;
using TGP.Seguridad.BussinessLogic.Config;

namespace TGP.Seguridad
{
    // Nota: para obtener instrucciones sobre cómo habilitar el modo clásico de IIS6 o IIS7, 
    // visite http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Propiedad que se utiliza para el logue de accion de la aplicación.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));

        /// <summary>
        /// Metodo que se inicializa la aplicacion
        /// </summary>
        protected void Application_Start()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
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

        /// <summary>
        /// Evento que maneja los errores de la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(Object sender, EventArgs e)
        {

            try
            {
                //Instanciar la excepcion de error
                System.Exception Error = Server.GetLastError();
                log.Error(Error.Message);
                Response.Clear();
                Server.ClearError();
                var httpException = Error as HttpException;

                if (httpException != null)
                {
                    Response.StatusCode = httpException.GetHttpCode();
                    switch (Response.StatusCode)
                    {
                        case 404:
                            if (Session["usuario"] == null)
                                Response.Redirect(ConfigurationManager.AppSettings["LoginURL"]);
                            else
                                Response.Redirect("~/Home/Index");
                            break;
                    }
                }
                else
                {
                    if (ConfigurationManager.AppSettings["SI_ENVIAR_EMAIL_ERROR"] == "true")
                    {
                        // Obtener el ultimo error 
                        String lastError = Error.Message;
                        // Enviar informacion por mail
                        MailMessage message = new MailMessage();
                        message.From = new MailAddress(ConfigurationManager.AppSettings["EMAIL_FROM"]);
                        message.To.Add(ConfigurationManager.AppSettings["EMAIL_TO"]);
                        // Nombre del proyecto
                        string projectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                        // Fin Nombre del proyecto
                        message.Subject = "[" + projectName + " - ERROR]";
                        #region get client info
                        System.Web.HttpBrowserCapabilities browser = Request.Browser;
                        string s = "Browser Capabilities\n"
                            + "Type = " + browser.Type + "\n"
                            + "Name = " + browser.Browser + "\n"
                            + "Version = " + browser.Version + "\n"
                            + "Major Version = " + browser.MajorVersion + "\n"
                            + "Minor Version = " + browser.MinorVersion + "\n"
                            + "Platform = " + browser.Platform + "\n"
                            + "Is Beta = " + browser.Beta + "\n"
                            + "Is Crawler = " + browser.Crawler + "\n"
                            + "Is AOL = " + browser.AOL + "\n"
                            + "Is Win16 = " + browser.Win16 + "\n"
                            + "Is Win32 = " + browser.Win32 + "\n"
                            + "Supports Frames = " + browser.Frames + "\n"
                            + "Supports Tables = " + browser.Tables + "\n"
                            + "Supports Cookies = " + browser.Cookies + "\n"
                            + "Supports VBScript = " + browser.VBScript + "\n"
                            + "Supports JavaScript = " +
                                browser.EcmaScriptVersion.ToString() + "\n"
                            + "Supports Java Applets = " + browser.JavaApplets + "\n"
                            + "Supports ActiveX Controls = " + browser.ActiveXControls
                                  + "\n";
                        #endregion
                        message.IsBodyHtml = true;
                        message.Body = "En la fecha <b>" + DateTime.Now.ToString() + "</b> ocurrio un error en la aplicación <b>[" + projectName + "]</b> <ul>";
                        if (Session["usuario"] != null)
                            message.Body += "<li><b>Usuario autenticado:</b> <b>[" + Session["usuario"].ToString() + "]</b> </li>";
                        message.Body += "<li><b>Datos del Error: </b>" + lastError + "</li>";
                        message.Body += "<li><b>Información del Cliente:</b>" + s + "</li>";
                        message.Body += "<li><b>Nombre del Servidor: </b> <b>[" + ConfigurationManager.AppSettings["serverLocation"] + "]</b></li></ul>";

                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["EMAIL_SMPT"]);
                        smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EMAIL_NETWORK_CREDENTIAL_USERNAME"], ConfigurationManager.AppSettings["EMAIL_NETWORK_CREDENTIAL_PASSWORD"]);
                        smtp.Send(message);
                    }
                }
            }
            catch (Exception eMail)
            {
                throw new Exception("Problemas al enviar: " + eMail.Message);
            }
            if (Session["usuario"] == null)
                Response.Redirect(ConfigurationManager.AppSettings["LoginURL"]);
            else
                Response.Redirect("~/Home/Index");
        }
    }
}