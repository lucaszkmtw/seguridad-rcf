using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using TGP.Seguridad.SRSeguridad;

namespace TGP.Seguridad.Controllers
{

    public class WebSecurityController : Controller
    {

        /// <summary>
        /// Metodo que envia los errores por email
        /// </summary>
        /// <param name="programmerError"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public EmptyResult EnviarEmail(string programmerError, string controllerName, string actionName, string descripcionFormulario, string usuario)
        {
            if (ConfigurationManager.AppSettings["SI_ENVIAR_EMAIL_ERROR"] == "true")
            {
                // Enviar informacion por mail
                MailMessage message = new MailMessage();
                message.From = new MailAddress(ConfigurationManager.AppSettings["EMAIL_FROM"]);
                message.To.Add(ConfigurationManager.AppSettings["EMAIL_TO"]);
                // Nombre del proyecto
                string projectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                // Fin Nombre del proyecto
                message.Subject = "[" + projectName + " - ERROR]";
                #region get client info
                System.Web.HttpBrowserCapabilitiesBase browser = Request.Browser;
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
                if (usuario != null)
                    message.Body += "<li><b>Usuario autenticado:</b> <b>[" + usuario + "]</b> </li>";
                message.Body += "<li><b>Datos del Error: </b>" + programmerError + "</li>";
                message.Body += "<li><b>Controller: </b>" + controllerName + "</li>";
                message.Body += "<li><b>Action: </b>" + actionName + "</li>";
                message.Body += "<li><b>Información del Cliente:</b>" + s + "</li>";
                message.Body += "<li><b>Nombre del Servidor: </b> <b>[" + ConfigurationManager.AppSettings["serverLocation"] + "]</b></li></ul>";
                if (!string.IsNullOrEmpty(descripcionFormulario))
                    message.Body += "<li><b>Descripción opcional del formulario: </b>" + descripcionFormulario + "</li>";
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["EMAIL_SMPT"]);
                smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EMAIL_NETWORK_CREDENTIAL_USERNAME"], ConfigurationManager.AppSettings["EMAIL_NETWORK_CREDENTIAL_PASSWORD"]);
                smtp.Send(message);
            }
            return new EmptyResult();
        }

        internal List<string> nodosPorActividad(String codActividad)
        {
            try
            {
                if (HttpContext.Session["usuario"] == null || HttpContext.Session["actividades"] == null)
                {
                    throw new TimeoutException("La sesión ha caducado.");
                }
                List<string> result = new List<string>();
                List<PermisoUsuarioWS> actividadesHabilitadas = (List<PermisoUsuarioWS>)HttpContext.Session["actividades"];
                actividadesHabilitadas = actividadesHabilitadas.Where(a => a.CodActividad == codActividad).ToList<PermisoUsuarioWS>();
                if (actividadesHabilitadas != null && actividadesHabilitadas.Count > 0)
                {
                    return actividadesHabilitadas.First().NodosAutorizados.ToList().Select(n => n.Codigo).ToList();

                }
                return result;
            }
            catch (Exception e)
            {
                return null;

            }

        }

        internal bool EsAmbienteDESACG()
        {
            return Session["ambienteDB"].ToString() == System.Configuration.ConfigurationManager.AppSettings["AMBIENTE_DESACG"];
        }

    }
}
