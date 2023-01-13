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
    public class NodoFuncionalController : WebSecurityController
    {
        protected NodoFuncionalApplicationService nodosService = NodoFuncionalApplicationService.Instance;


        /// <summary>
        /// Metodo que nos lleva al listado de nodos
        /// </summary>
        /// <returns></returns>
        [Autoriza("listarNodoFuncional")]
        public ActionResult Listado()
        {
            ViewBag.Estructuras = new SelectList(nodosService.GetComboEstructuras(true), "Codigo", "Descripcion");
            ViewBag.EsAmbienteDESACG = EsAmbienteDESACG();
            return View(new List<ListadoNodoFuncionalViewModel>());
        }


        /// <summary>
        /// Metodo que llama al servicio para obtener los nodos segun estructura
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        public ActionResult GetNodos(string cod)
        {
            Session["estructuraSelected"] = cod;
            IList<ListadoNodoFuncionalViewModel> nodos = nodosService.GetNodos(cod);
            return PartialView("Partials/_ResultadoListadoNodosFuncionales", nodos);
        }

        /// <summary>
        /// Metodo que lleva a la pantalla de Nuevo Nodo
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        [Autoriza("nuevoNodoFuncional")]
        public ActionResult Nuevo(string cod)
        {
            //if (EsAmbienteDESACG())
            //{
                NodoFuncionalDTO model = new NodoFuncionalDTO();
                if (cod != ComboGenerico.ComboVacio)
                {
                    model.EstructuraFuncional = cod;
                    ViewBag.NodosPadre = GetComboNodosPadre(cod);
                }
                else
                {
                    ViewBag.NodosPadre = new SelectList(new List<ComboGenerico>(), "Codigo", "Descripcion");
                }
                ViewBag.TipoNodo = new SelectList(nodosService.GetTiposNodo(), "Codigo", "Descripcion");
                ViewBag.Estructuras = new SelectList(nodosService.GetComboEstructuras(true), "Codigo", "Descripcion");
                return View("NuevoNodo", model);
           // }

            //return Redirect("~/NodoFuncional/Listado");
        }

        /// <summary>
        /// Metodo que lleva a la pantalla de Edicion de Nodo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Autoriza("editarNodoFuncional")]
        public ActionResult Editar(long id)
        {
            NodoFuncionalDTO nodoEditar = nodosService.GetNodoEditar(id);
            ViewBag.NodosPadre = GetComboNodosPadre(nodoEditar.EstructuraFuncional);
            ViewBag.TipoNodo = new SelectList(nodosService.GetTiposNodo(), "Codigo", "Descripcion");
            ViewBag.Estructuras = new SelectList(nodosService.GetComboEstructuras(true), "Codigo", "Descripcion");
            return View("EditarNodo", nodoEditar);
        }

        /// <summary>
        /// Metodo que llama al servicio para guardar un nodo editado
        /// </summary>
        /// <param name="nodo"></param>
        /// <returns></returns>
        [Autoriza("editarNodoFuncional"), HttpPost]
        public ActionResult GuardarEdicionNodo(NodoFuncionalDTO nodo)
        {
            string result = nodosService.GuardarEdicionNodo(nodo);
            if (result != string.Empty)
                return new JsonResult { Data = new { mensaje = result, tipoMensaje = "info" } };
            else
                return new JsonResult { Data = new { mensaje = "El Nodo se ha guardado exitosamente.", tipoMensaje = "success" } };
        }

        /// <summary>
        /// Metodo que llama al servicio para verificar si el nodo funcional ya se encuentra en el ambiente destino
        /// </summary>
        /// <param name="nodo"></param>
        /// <returns></returns>
        [HttpPost, Autoriza("nuevoNodoFuncional")]
        public JsonResult PrevGuardarNuevoNodo(NodoFuncionalDTO nodo)
        {
            string mjs = nodosService.PrevGuardarNuevoNodo(nodo);
            return new JsonResult { Data = new { mensaje = mjs, tipoMensaje = "info" } };
        }

        /// <summary>
        /// Metodo que llama al servicio para guardar un nuevo nodo
        /// </summary>
        /// <param name="nodo"></param>
        /// <returns></returns>
        [Autoriza("nuevoNodoFuncional"), HttpPost]
        public JsonResult GuardarNuevoNodo(NodoFuncionalDTO nodo)
        {

            string result = nodosService.GuardarNuevoNodo(nodo);
            if (result != string.Empty)
                return new JsonResult { Data = new { mensaje = result, tipoMensaje = "info" } };
            else
                return new JsonResult { Data = new { mensaje = "El Nodo se ha guardado exitosamente.", tipoMensaje = "success" } };

        }

        /// <summary>
        /// Metodo que llama al servicio para eliminar un nodo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [Autoriza("eliminarNodoFuncional"), HttpPost]
        public JsonResult Eliminar(long id, int version)
        {
            nodosService.EliminarNodo(id, version);
            return new JsonResult { Data = new { mensaje = "El Nodo ha sido eliminado.", tipoMensaje = "success" } };
        }


        /// <summary>
        /// Metodo que devuelve a la vista los nodos padre para el combo de seleccion
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        public JsonResult GetNodosPadre(string cod)
        {
            return new JsonResult { Data = new { Nodos = GetComboNodosPadre(cod) } };
        }


        /// <summary>
        /// Metodo que obtiene los posibles nodos padre
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <returns></returns>
        private SelectList GetComboNodosPadre(string codigoEstructura = null)
        {
            List<ComboGenerico> nodosPadre = new List<ComboGenerico>()
            {
                new ComboGenerico()
                {
                    Codigo = "-1",
                    Id = -1,
                    Descripcion = "-- Sin Nodo Padre --"
                }
            };
            if (codigoEstructura != ComboGenerico.ComboVacio)
            {
                nodosPadre.AddRange(nodosService.GetComboNodos(codigoEstructura));
            }
            return new SelectList(nodosPadre, "Codigo", "Descripcion");
        }
    }
}