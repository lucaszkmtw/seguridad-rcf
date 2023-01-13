using ErrorManagerTGP.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGP.Seguridad.BussinessLogic;
using TGP.Seguridad.BussinessLogic.Dto;

namespace TGP.Seguridad.Controllers
{
    public class MenuOpcionController : WebSecurityController
    {
        protected MenuOpcionApplicationService menuService = MenuOpcionApplicationService.Instance;

        [Autoriza("listarMenuOpcion")]
        public ActionResult Listado()
        {
            ViewBag.Estructuras = new SelectList(menuService.GetComboEstructuras(true), "Codigo", "Descripcion");
            ViewBag.EsAmbienteDESACG = EsAmbienteDESACG();
            return View(new List<ListadoMenuOpcionViewModel>());
        }

        /// <summary>
        /// Metodo que llama al servicio para obtener los menues segun una estructura
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        [Autoriza("listarMenuOpcion")]
        public ActionResult GetMenues(string cod)
        {
            Session["estructuraSelected"] = cod;
            IList<ListadoMenuOpcionViewModel> menues = menuService.GetMenues(cod);
            return PartialView("Partials/_ResultadoListadoMenues", menues);
        }


        /// <summary>
        /// Metodo que nos lleva al form de menu para crear uno nuevo
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        [Autoriza("nuevaMenuOpcion")]
        public ActionResult Nuevo(string cod)
        {

            ViewBag.Estructuras = new SelectList(menuService.GetComboEstructuras(), "Codigo", "Descripcion");
            ViewBag.Actividades = new SelectList(new List<ComboGenerico>(), "Codigo", "Descripcion");
            ViewBag.MenuesPadre = new SelectList(new List<ComboGenerico>(), "Codigo", "Descripcion");
            MenuOpcionDTO model = new MenuOpcionDTO();
            if (cod != ComboGenerico.ComboVacio)
                model.EstructuraFuncional = cod;
            return View("NuevoMenu", model);
        }



        /// <summary>
        /// Metodo que nos lleva al form de menu para editar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Autoriza("editarMenuOpcion")]
        public ActionResult Editar(long id)
        {
            MenuOpcionDTO menuOpcionEditar = menuService.GetMenuEditar(id);
            ViewBag.Estructuras = new SelectList(menuService.GetComboEstructuras(), "Codigo", "Descripcion");
            ViewBag.Actividades = GetComboActividadesMenu(menuOpcionEditar.EstructuraFuncional);
            ViewBag.MenuesPadre = GetComboMenuesPadre(menuOpcionEditar.EstructuraFuncional);
            return View("EditarMenu", menuOpcionEditar);
        }


        /// <summary>
        /// Metodo que devuelve los combos correspondientes al form de Menu Opcion
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        public JsonResult GetCombosFormMenu(string cod)
        {
            //Obtiene el codigo del nuevo menu segnu la estructura
            long codigoNuevoMenu = menuService.GetCodigoNuevoMenu(cod);
            SelectList actividades = GetComboActividadesMenu(cod);
            SelectList menuesPadre = GetComboMenuesPadre(cod);
            return new JsonResult { Data = new { Actividades = actividades, Menues = menuesPadre, CodigoNuevoMenu = codigoNuevoMenu } };
        }

        /// <summary>
        /// Metodo que llama al servicio para verificar si el menu ya se encuentra en el ambiente destino
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost, Autoriza("nuevaMenuOpcion")]
        public JsonResult PrevGuardarNuevoMenu(MenuOpcionDTO menu)
        {
            string mjs = menuService.PrevGuardarNuevoMenu(menu);
            return new JsonResult { Data = new { mensaje = mjs, tipoMensaje = "info" } };
        }

        /// <summary>
        /// Metodo que llama al servicio para guardar un nuevo menu
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [Autoriza("nuevaMenuOpcion"), HttpPost]
        public JsonResult GuardarNuevoMenu(MenuOpcionDTO menu)
        {
            menuService.GuardarNuevoMenu(menu);
            return new JsonResult { Data = new { mensaje = "El Menú fue guardado exitosamente.", tipoMensaje = "success" } };

        }

        /// <summary>
        /// Metodoq ue llama al servicio para guarda una edicion de menu
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [Autoriza("editarMenuOpcion"), HttpPost]
        public JsonResult GuardarEdicionMenu(MenuOpcionDTO menu)
        {
            menuService.GuardarEdicionMenu(menu);
            return new JsonResult { Data = new { mensaje = "El Menú fue guardado exitosamente.", tipoMensaje = "success" } };

        }

        /// <summary>
        /// Método que llama al servicio para eliminar una opcion de menu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [Autoriza("eliminarMenuOpcion"), HttpPost]
        public JsonResult EliminarMenu(long id, int version)
        {

            menuService.EliminarMenu(id, version);
            return new JsonResult { Data = new { mensaje = "El Menú fue eliminado exitosamente.", tipoMensaje = "success" } };
        }


        #region Metodos Privados
        /// <summary>
        /// Metodo qe devuelveun select list de las actividades correspondientes a una estructura con opcion sin actividad
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <returns></returns>
        private SelectList GetComboActividadesMenu(string codigoEstructura)
        {
            //Listado de actividades segun estructura
            List<ComboGenerico> listadoActividades = new List<ComboGenerico>()
            {
                new ComboGenerico()
                {
                    Codigo = "0",
                    Descripcion = "-- Sin Actividad --",
                    Id = 0
                }
            };
            //Agregamos al campo vacio el resto
            listadoActividades.AddRange(menuService.GetComboActividades(codigoEstructura));

            return new SelectList(listadoActividades, "Codigo", "Descripcion");
        }

        /// <summary>
        /// Metodo qe devuelveun select list de los menues correspondientes a una estructura con opcion sin padre
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <returns></returns>
        private SelectList GetComboMenuesPadre(string codigoEstructura)
        {
            //Listado de menues segun estructura
            List<ComboGenerico> listadoMenues = new List<ComboGenerico>()
            {
                new ComboGenerico()
                {
                    Codigo = "0",
                    Descripcion = "-- Sin Padre --",
                    Id = 0
                }
            };
            //Agregamos al campo vacio el resto de los menues
            listadoMenues.AddRange(menuService.GetComboMenues(codigoEstructura));

            return new SelectList(listadoMenues, "Codigo", "Descripcion");
        }
        #endregion

    }
}