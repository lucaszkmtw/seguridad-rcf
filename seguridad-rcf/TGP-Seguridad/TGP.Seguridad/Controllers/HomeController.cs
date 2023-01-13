using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TGP.Seguridad.BussinessLogic;
using TGP.Seguridad.BussinessLogic.APIResponses;
using TGP.Seguridad.BussinessLogic.Dto;

namespace TGP.Seguridad.Controllers
{
    #region //Referencias a las capas //
    #endregion

    

    public class HomeController : WebSecurityController
    {

        protected SeguridadApplicationService service = SeguridadApplicationService.Instance;

        #region // Metodos Publicos //


        public HomeController() { }
        /// <summary>
        /// Metodo que muestra la vista por defecto de la aplicacion
        /// </summary>
        /// <returns></returns>
        //Ejemplo de utilizacion de atributo [Autoriza("crearAplicacion")]
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                Session.Clear();
                return RedirectToAction("Login", "Account");
            }
            else
            {
                //Recupero las novedades
                ObtenerNovedades4API();
                //string ActionSiguiente = service.MarcarUsuarioInactivo(23382);
                return View();
            }
        }


        [HttpPost]
        public ActionResult GetNovedades()
        {
            ObtenerNovedades4API();
            return PartialView("~/Views/Shared/Partials/CardContentNovedades.cshtml");
        }

        public ActionResult TestIndex()
        {
            return View();
        }

        public ActionResult IrPortalListaNovedades(string selected = "")
        {
            if(string.IsNullOrEmpty(selected))
                return Redirect(System.Configuration.ConfigurationManager.AppSettings["PORTAL_LISTADO_NOVEDADES"].ToString());
            else
                return Redirect(System.Configuration.ConfigurationManager.AppSettings["PORTAL_LISTADO_NOVEDADES"].ToString()+ "?selected=" + selected);
        }

        [HttpGet]
        public ActionResult GetApps()
        {
            ObtenerAppsAPI();
            return PartialView("~/Views/Shared/Partials/_LayoutAplicaciones.cshtml");
        }



        #endregion

        #region // Metodos Privados //

        //TODO: Para un mejor desarrollo ver las buenas practicas que se encuentra en la carpeta documentación


        /// <summary>
        /// Metodo que recupera las novedades del usuario autenticado mediante una API
        /// </summary>
        private void ObtenerNovedades4API()
        {
            try
            {
                var novedades4Api = service.ObtenerNovedades(Session["usuario"].ToString());
                List<NovedadDTO> novedadesNoLeidas = novedades4Api.Item1;
                List<NovedadDTO> advertenciasTotalesVigentes = novedades4Api.Item2;
                ViewBag.CantidadNovedadesNoLeidas = novedadesNoLeidas.Count;
                ViewBag.NovedadesAdvertenciasNoLeidas = novedades4Api.Item3.Count;
                ViewBag.AdvertenciasTotalesVigentes = advertenciasTotalesVigentes;
                ViewBag.UltimasNovedadesNoLeidas = novedadesNoLeidas.Take(5).ToList();
                ViewBag.MensajeError = null;
            }
            catch (Exception ex)
            {
                @ViewBag.CantidadNovedadesNoLeidas = "[ ! ]";
                ViewBag.MensajeError = ex.Message;
            }
        }


        //TODO: Para un mejor desarrollo ver las buenas practicas que se encuentra en la carpeta documentación


        // se agrega meto para recuperar apps mas usadas 

        private void ObtenerAppsAPI()
        {
            string usuario = Session["usuario"] != null ? Session["usuario"].ToString() : "";
            if (Session["usuario"] != null)
            {
                List<ResponseAppsMasUsadas> apps = service.GetAppsMasUsadas(usuario, "SEG");
                ViewBag.Apps = (apps.Count > 0) ? apps : null;
            }
        }




        #endregion


    }
}