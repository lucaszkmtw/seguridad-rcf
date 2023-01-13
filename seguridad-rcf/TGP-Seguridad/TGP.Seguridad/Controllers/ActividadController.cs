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
    public class ActividadController : WebSecurityController
    {
        protected ActividadApplicationService actividadService = ActividadApplicationService.Instance;

        /// <summary>
        /// Metodo que nos lleva al listado de actividades
        /// </summary>
        /// <param name="est"></param>
        /// <returns></returns>
        [Autoriza("listarActividad")]
        public ActionResult Listado(string est = ComboGenerico.ComboVacio)
        {
            ViewBag.Estructuras = new SelectList(actividadService.GetComboEstructuras(true), "Codigo", "Descripcion");
            ViewBag.EstructuraSelected = est;
            ViewBag.EsAmbienteDESACG = EsAmbienteDESACG();
            return View(new List<ListadoActividadViewModel>());
        }

        /// <summary>
        /// Metodo que obtiene las actividades a listar segun estructura funcional
        /// </summary>
        /// <param name="cod">Codigo de Estructura Funcional</param>
        /// <returns></returns>
        [HttpPost, Autoriza("listarActividad")]
        public ActionResult GetActividades(string cod)
        {
            Session["estructuraSelected"] = cod;
            IList<ListadoActividadViewModel> actividades = actividadService.GetActividades(cod);
            return PartialView("Partials/_ResultadoListadoActividades", actividades);
        }

        /// <summary>
        /// Metodo que nos lleva al form para crear una nueva actividad
        /// </summary>
        /// <param name="est">Codigo de estructura en que estaba el listado</param>
        /// <returns></returns>
        [Autoriza("nuevaActividad")]
        public ActionResult Nueva(string cod)
        {
            //if (EsAmbienteDESACG())
            //{
                ViewBag.Estructuras = new SelectList(actividadService.GetComboEstructuras(), "Codigo", "Descripcion");
                ActividadDTO model = new ActividadDTO();

                //Si el valor no es vacio seteamos el combo de estructura igual que como dejo el del listado
                if (cod != ComboGenerico.ComboVacio)
                    model.EstructuraFuncional = cod;

                return View("NuevaActividad", model);
            //}

            //return Redirect("~/Actividad/Listado");
        }

        /// <summary>
        /// Metodo que nos lleva al form para editar una activdiad
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Autoriza("editarActividad")]
        public ActionResult Editar(long id)
        {
            ViewBag.Estructuras = new SelectList(actividadService.GetComboEstructuras(), "Codigo", "Descripcion");
            return View("EditarActividad", actividadService.GetActividadEditar(id));
        }

        /// <summary>
        /// Metodo que llama al servicio para verificar si la actividad ya se encuentra en el ambiente destino
        /// </summary>
        /// <param name="actividad"></param>
        /// <returns></returns>
        [HttpPost, Autoriza("nuevaActividad")]
        public JsonResult PrevGuardarNuevaActividad(ActividadDTO actividad)
        {
            string mjs = actividadService.PrevGuardarNuevaActividad(actividad);
            return new JsonResult { Data = new { mensaje = mjs, tipoMensaje = "info" } };
        }

        /// <summary>
        /// Metodo que llama al servicio para guardar una nueva actividad
        /// </summary>
        /// <param name="actividad"></param>
        /// <returns></returns>
        [HttpPost, Autoriza("nuevaActividad")]
        public JsonResult GuardarNuevaActividad(ActividadDTO actividad)
        {
            actividadService.GuardarNuevaActividad(actividad);
            return new JsonResult { Data = new { mensaje = "La Actividad fue creada con exito.", tipoMensaje = "success" } };
        }

        /// <summary>
        /// Metodo que llama al servicio para guardar una edicion de una actividad
        /// </summary>
        /// <param name="actividad"></param>
        /// <returns></returns>
        [HttpPost, Autoriza("editarActividad")]
        public JsonResult GuardarEdicionActividad(ActividadDTO actividad)
        {

            actividadService.GuardarEdicionActividad(actividad);
            return new JsonResult { Data = new { mensaje = "La Actividad fue editada con exito.", tipoMensaje = "success" } };
        }

        /// <summary>
        /// Metodo que llama el servicio para eliminar una actividad
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpPost, Autoriza("eliminarActividad")]
        public JsonResult EliminarActividad(long id, int version)
        {
            actividadService.EliminarActividad(id, version);
            return new JsonResult { Data = new { mensaje = "La Actividad fue eliminada con exito.", tipoMensaje = "success" } };
        }



    }
}