using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Configuration;
using TGP.Seguridad.SRSeguridad;
using TGP.Seguridad.Forms;
using ErrorManagerTGP.Attributes;
using TGP.Seguridad.BussinessLogic;
using System.Net;
using System.IO;
using System.Web;
using System.Web.Configuration;

namespace TGP.Seguridad.Controllers
{

    public class AccountController : WebSecurityController
    {
        #region //Variables Privadas

        //Codigo correspondiente a la estructura funcional de este proyecto
        private static string APPEstructura = "SEG"; //Variable que debe cambiar;
        private static SeguridadApplicationService seguridadAppBussinessLogicService = SeguridadApplicationService.Instance;
        #endregion

        #region //Metodos Publicos

        /// <summary>
        /// Metodo que muestra la vista login de acceso a la aplicacion, de visiblidad para todos los usuarios
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [PermitirAnonimo]
        public ActionResult Login(string returnUrl)
        {
            // se recupera la version de recursos
            string nuevaVersion = "";
            //WebRequest request = WebRequest.Create(ConfigurationManager.AppSettings["WebResources"] + "~/ResourceDesign/Version.txt");
            //WebRequest request = WebRequest.Create("~/ResourceDesign/Version.txt");

            //using (WebResponse response = request.GetResponse())
            //{
            //    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            //    {
            //        nuevaVersion = stream.ReadToEnd();
            //    }
            //}
            Session["version"] = nuevaVersion;

            if (Session["usuario"] == null)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Metodo de autenticacion de la aplicacion
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [PermitirAnonimo]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UsuarioForm model, string returnUrl)
        {
            LibreriaSeguridadClient proxy = new LibreriaSeguridadClient();

            if (ModelState.IsValid)
            {
                Session["token"] = seguridadAppBussinessLogicService.SolicitarToken();

                LoginRequest loginRq = new LoginRequest(model.usuario, model.clave, APPEstructura, Utils.Encriptador.Encriptar(Session["token"].ToString()) );
                LoginResponse r = proxy.Login(loginRq);
                if (r.LoginResult.Valido)
                {
                    CargarContexto(model.usuario, r.LoginResult);

                    return RedirectToAction("Index", "Home");
                }
                TempData["ErrorLogin"] = r.LoginResult.Msj;
                if (r.LoginResult.Exception != null)//envio mail con la excepcion que devolvio SRSeguridad
                {
                    string excepcion = "Excepcion producida por SRSeguridad: " + r.LoginResult.Exception;
                    string descripcionError = "Envio de mail automatico para informar excepcion producida por WS.";
                    EnviarEmail(excepcion, "AccountController", "Login", descripcionError, null);
                }
            }
            return View(model);
        }

        /// <summary>
        /// Metodo invocado desde el portal
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [PermitirAnonimo]
        public ActionResult ExternalLogin(string usuario, string returnUrl)
        {
            UsuarioForm usuarioModel = new UsuarioForm();
            if (usuario == HttpContext.User.Identity.Name && HttpContext.User.Identity.IsAuthenticated)
            {
                Session["token"] = seguridadAppBussinessLogicService.SolicitarToken();
                return ExternalLoginService(usuario, returnUrl, Utils.Encriptador.Encriptar(Session["token"].ToString()) );
            }
            string url = ConfigurationManager.AppSettings["PortalURL"];
            return Redirect(url);
        }

        /// <summary>
        /// Metodo que permite la autenticacion externa
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [PermitirAnonimo]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginService(string usuario, string returnUrl, string token)
        {
            LibreriaSeguridadClient proxy = new LibreriaSeguridadClient();

            ExternalLoginRequest loginRq = new ExternalLoginRequest(usuario, APPEstructura, token);
            ExternalLoginResponse r = proxy.ExternalLogin(loginRq);
            if (r.ExternalLoginResult.Valido)
            {
                CargarContexto(usuario, r.ExternalLoginResult);

                return RedirectToAction("Index", "Home");
            }
            TempData["ErrorLogin"] = r.ExternalLoginResult.Msj;
            if (r.ExternalLoginResult.Exception != null)//envio mail con la excepcion que devolvio SRSeguridad
            {
                string excepcion = "Excepcion producida por SRSeguridad: " + r.ExternalLoginResult.Exception;
                string descripcionError = "Envio de mail automatico para informar excepcion producida por WS.";
                EnviarEmail(excepcion, "AccountController", "ExternalLoginService", descripcionError, usuario);
            }
            return View("Login");
        }


        private void CargarContexto(string usuario, ResultadoLogin r)
        {
           
            //TODO cargo las actividades asociadas al usuario para el acceso a menu y seguridad
            FormsAuthentication.SetAuthCookie(usuario, false);
            Session.Add("usuario", usuario);
            if (r.Avatar != null)
                Session.Add("avatar", r.Avatar);
            Session.Add("usuarioId", r.UsuarioId);
            Session.Add("EsAdmin", UsuarioApplicationService.EsUsuarioAdmin());
            Session.Add("CodigosActividades", ObtenerCodigosActividades(r.Permisos.ToList()));
            Session.Add("actividades", r.Permisos.ToList());
            Session.Add("menues", r.Menues.ToList());
            Session.Add("ambienteDB", seguridadAppBussinessLogicService.ObtenerNombreBaseDeDatos());
            Session.Add("estructuraSelected", "0");
            EnviarActividadMonitor(true);
        }


        /// <summary>
        /// Método que obtiene los códigos de actividades de la lista de permisos del usuario
        /// </summary>
        /// <param name="actividades">Lista de Actividades a procesar</param>
        /// <returns>lista de string con los codigos de las actividades </returns>
        private List<String> ObtenerCodigosActividades(List<PermisoUsuarioWS> actividades)
        {
            List<String> listaCodigoActividades = new List<string>();
            foreach (var a in actividades)
            {
                listaCodigoActividades.Add(a.CodActividad);
            }
            return listaCodigoActividades;
        }


        /// <summary>
        /// Metodo que permite la carga del menu
        /// </summary>
        /// <param name="usuariosMenu"></param>
        /// <returns></returns>
        public static TreeView LoadTreeMenu(List<Seguridad.SRSeguridad.MenuWS> usuariosMenu)
        {
            TreeView tvMenu = new TreeView();
            if (usuariosMenu != null && usuariosMenu.Count > 0)
            {
                foreach (Seguridad.SRSeguridad.MenuWS menu in usuariosMenu.Where(m => m.IdPadre == null).ToList())
                {
                    TreeNode ParentNode = new TreeNode();
                    ParentNode.Text = menu.Descripcion;
                    ParentNode.NavigateUrl = menu.Url;
                    ParentNode.ImageUrl = menu.Icono;
                    tvMenu.Nodes.Add(ParentNode);
                    LoadTreeSubMenu(ref ParentNode, usuariosMenu.Where(m => m.IdPadre == menu.Id).ToList(), usuariosMenu);
                }
            }
            return tvMenu;
        }

        /// <summary>
        /// Metodo que permite el deslogeo de la aplicacion
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermitirAnonimo]
        public ActionResult LogOff()
        {
            try
            {
       //         EnviarActividadMonitor(false);
                Session.Clear();
                FormsAuthentication.SignOut();
                
                //Abandono de la sesión del usuario
                Session.Abandon();

                // limpia la cookie de autenticación
                HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                cookie1.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie1);

                // limpia la cookie de sessionState
                SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
                HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
                cookie2.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie2);

                //Y luego se redirecciona al login 
                return Redirect(FormsAuthentication.LoginUrl);

            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return View("Error");
            }
        }

        /// <summary>
        /// Metodo que muestra la vista Acerca De
        /// </summary>
        /// <returns></returns>
        public ActionResult AcercaDe()
        {
            ViewBag.Version = ConfigurationManager.AppSettings["version"];
            ViewBag.DateVersion = ConfigurationManager.AppSettings["dateVersion"];
            ViewBag.Server = ConfigurationManager.AppSettings["serverLocation"];
            return View();
        }

        #endregion

        #region //Metodos Privados//

        /// <summary>
        /// Método que comunica la actividad del usuario a Seguridad/Monitor
        /// </summary>
        /// <param name="siConexion"></param>
        private void EnviarActividadMonitor(bool siConexion)
        {
            try
            {
                if (siConexion == true)
                {
                    //Registracion de Auditoria
                    RegistarAuditoriaConexion();
                }
                else
                {
                    //Registracion de Auditoria
                    RegistarAuditoriaDesconexion();
                }
            }
            catch (Exception eActividad)
            {
                throw new Exception("Se produjo un problema con el envio de Auditoria de Conexion: " + eActividad.Message);

            }
        }

        /// <summary>
        /// Método que registra una conexión a la aplicación
        /// </summary>
        private void RegistarAuditoriaConexion()
        {
            //Browser con el cual se conecto
            var browser = HttpContext.Request.Browser.Browser;
            if (browser != null && browser.Length > 300)
            {
                browser = browser.Substring(0, 300);
            }
            //Instancia del Cliente de Seguridad
            SRSeguridad.LibreriaSeguridadClient client = new LibreriaSeguridadClient();
            //Carga del request de auditoria
            SRSeguridad.RegistarAuditoriaConexionRequest request = new RegistarAuditoriaConexionRequest();
            if (Session["avatar"] != null)
                request.avatar = (byte[])Session["avatar"];
            else
                request.avatar = null;
            request.browserCliente = browser;
            request.nombreUsuario = Convert.ToString(Session["usuario"]);
            request.aplicacion = APPEstructura;
            request.IPCliente = GetIP();
            request.usuarioID = Convert.ToInt64(Session["usuarioId"]);
            request.serverDescripcion = ConfigurationManager.AppSettings["serverLocation"];
            request.token = Utils.Encriptador.Encriptar(Session["token"].ToString());
            //Registrar
            client.RegistarAuditoriaConexion(request);
        }

        /// <summary>
        /// Método que registra cuando se desconecta de la aplicación
        /// </summary>
        private void RegistarAuditoriaDesconexion()
        {
            //Browser con el cual se conecto
            var browser = HttpContext.Request.Browser.Browser;
            if (browser != null && browser.Length > 300)
            {
                browser = browser.Substring(0, 300);
            }
            //Instancia del Cliente de Seguridad
            SRSeguridad.LibreriaSeguridadClient client = new LibreriaSeguridadClient();
            //Carga del request de auditoria
            SRSeguridad.RegistarAuditoriaDesconexionRequest request = new RegistarAuditoriaDesconexionRequest();
            if (Session["avatar"] != null)
                request.avatar = (byte[])Session["avatar"];
            else
                request.avatar = null;
            request.browserCliente = browser;
            request.nombreUsuario = Convert.ToString(Session["usuario"]); ;
            request.aplicacion = APPEstructura;
            request.IPCliente = GetIP();
            request.usuarioID = Convert.ToInt64(Session["usuarioId"]);
            request.serverDescripcion = ConfigurationManager.AppSettings["serverLocation"];
            request.token = Utils.Encriptador.Encriptar(Session["token"].ToString());
            //Registrar
            client.RegistarAuditoriaDesconexion(request);
        }

        /// <summary>
        /// Método que recupera la IP por la cual está conectado el usuario
        /// </summary>
        /// <returns></returns>
        private string GetIP()
        {
            string ipaddress;
            ipaddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (ipaddress == "" || ipaddress == null)
            {
                ipaddress = Request.ServerVariables["REMOTE_ADDR"];
            }
            if (ipaddress == "" || ipaddress == null)
            {

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    ipaddress = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else if (System.Web.HttpContext.Current.Request.UserHostAddress.Length != 0)
                {
                    ipaddress = System.Web.HttpContext.Current.Request.UserHostAddress;
                }
            }
            return ipaddress;
        }


        /// <summary>
        /// Metodo que carga el sub menu de la aplicacion
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="hijosUsuariosMenu"></param>
        /// <param name="menu"></param>
        private static void LoadTreeSubMenu(ref TreeNode ParentNode, List<MenuWS> hijosUsuariosMenu, List<MenuWS> menu)
        {
            if (hijosUsuariosMenu.Count > 0)
            {
                foreach (MenuWS hijo in hijosUsuariosMenu)
                {
                    TreeNode child = new TreeNode();
                    child.Text = hijo.Descripcion;
                    child.NavigateUrl = hijo.Url;
                    ParentNode.ChildNodes.Add(child);
                    //Recursion Call
                    LoadTreeSubMenu(ref child, menu.Where(m => m.IdPadre == hijo.Id).ToList(), menu);
                }
            }
        }

        /// <summary>
        /// TODO:Redireccion Local
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                ViewBag.ErrorMessage = "URL Erronea.";
                return View("../Home/Error");
            }
        }

        #endregion
    }
}