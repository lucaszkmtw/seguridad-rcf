using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGP.Seguridad.BussinessLogic;
using TGP.Seguridad.BussinessLogic.Dto;

namespace TGP.Seguridad.Controllers
{
    public class EstructuraFuncionalController : WebSecurityController
    {
        protected EstructuraApplicationService estructuraService = EstructuraApplicationService.Instance;

        /// <summary>
        /// Metodoq ue nos lleva al listado dee structuras
        /// </summary>
        /// <returns></returns>
        public ActionResult Listado()
        {
            IList<EstructuraFuncionalDTO> estructuras = estructuraService.GetEstructuras(bool.Parse(Session["EsAdmin"].ToString()));
            ViewBag.EsAmbienteDESACG = EsAmbienteDESACG();
            return View(estructuras);
        }


        /// <summary>
        /// Metodo que nos lleva al form para una nueva estructura
        /// </summary>
        /// <returns></returns>
        public ActionResult Nuevo()
        {
           
                return View("NuevaEstructura", new EstructuraFuncionalDTO());
            
            //return Redirect("~/EstructuraFuncional/Listado");
        }


        /// <summary>
        /// Metodo que nos lleva al form para editar una estructura
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Editar(long id)
        {
            return View("EditarEstructura", estructuraService.GetEstructuraEditar(id));
        }


        /// <summary>
        /// Metodo que llama al servicio para verificar si la estructura funcional ya se encuentra en el ambiente destino
        /// </summary>
        /// <param name="estructura"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PrevGuardarNuevaEstructura(EstructuraFuncionalDTO estructura)
        {
            string mjs = estructuraService.PrevGuardarNuevaEstructura(estructura);
            return new JsonResult { Data = new { mensaje = mjs, tipoMensaje = "info" } };
        }

        /// <summary>
        /// Metodo que llama al servuicio para guardar una nuevaestructura
        /// </summary>
        /// <param name="estructura"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GuardarNuevaEstructura(EstructuraFuncionalDTO estructura)
        {

            estructuraService.GuardarNuevaEstructura(estructura);
            return new JsonResult { Data = new { mensaje = "La Estructura se ha guardado exitosamente.", tipoMensaje = "success" } };
        }

        /// <summary>
        /// Metodo que llama al servicio para editar una estructura
        /// </summary>
        /// <param name="estructura"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GuardarEdicionEstructura(EstructuraFuncionalDTO estructura)
        {

            estructuraService.GuardarEdicionEstructura(estructura);
            return new JsonResult { Data = new { mensaje = "La Estructura se ha guardado exitosamente.", tipoMensaje = "success" } };
        }


        /// <summary>
        /// Metodo que llama al servicio para eliminar una estructura
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Eliminar(long id, int version)
        {
            estructuraService.EliminarEstructura(id, version);
            return new JsonResult { Data = new { mensaje = "La Estructura ha sido eliminada.", tipoMensaje = "success" } };
        }
    }
}