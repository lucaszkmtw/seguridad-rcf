using ErrorManagerTGP.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGP.Seguridad.BussinessLogic;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.BussinessLogic.Generics;

namespace TGP.Seguridad.Controllers
{
    public class RolController : WebSecurityController
    {
        //protected SeguridadApplicationService service = SeguridadApplicationService.Instance;
        protected RolApplicationService rolService = RolApplicationService.Instance;

        #region ABM y Listado Roles
        [Autoriza("listarRol")]
        public ActionResult Listado(string est = null)
        {
            ViewBag.Estructuras = new SelectList(rolService.GetComboEstructuras(true), "Codigo", "Descripcion");
            if (est != null && est != ComboGenerico.ComboVacio)
                ViewBag.EstructuraSeleccionada = est;
            ViewBag.EsAmbienteDESACG = EsAmbienteDESACG();
            return View(new List<RolDTO>());
        }

        [Autoriza("listarRol")]
        public ActionResult GetRoles(string cod)
        {
            Session["estructuraSelected"] = cod;
            return PartialView("Partials/_ResultadoListadoRoles", rolService.GetRolesListadoViewModel(cod));
        }

        /// <summary>
        /// Metodo que devuelve las actividades segun la Estructura seleccionada
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        public JsonResult GetActiviadesPorEstructura(string cod)
        {
            if (cod.Equals("0"))
                return Json(new SelectList(new List<ComboGenerico>(), "Codigo", "Descripcion"));
            else
                return Json(new SelectList(rolService.GetComboActividades(cod), "Codigo", "Descripcion"));
        }

        /// <summary>
        /// Metodo que nos lleva al form de Rol para crear uno nuevo
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        [Autoriza("nuevoRol")]
        public ActionResult NuevoRol(string cod)
        {
            //if (EsAmbienteDESACG())
            //{
                RolDTO rol = new RolDTO();

                //Obtenemos las estructuras
                ViewBag.Estructuras = new SelectList(rolService.GetComboEstructuras(false), "Codigo", "Descripcion");
                ViewBag.TipoNodo = new SelectList(rolService.GetTiposNodo(), "Codigo", "Descripcion");
                //Si viene de buscar el listado de una estructura en especifico
                //obtenemos las actividades de esa estructura, sino instanciamos una lista vacia
                if (cod != null && !cod.Equals("0"))
                {
                    ViewBag.Actividades = rolService.GetComboActividades(cod);
                    rol.EstructuraFuncional = cod;
                }
                else
                    ViewBag.Actividades = new List<ComboGenerico>();

                return View(rol);
            //}

            //return Redirect("~/Rol/Listado");
        }

        /// <summary>
        /// Metodo que llama al servicio para verificar si el rol ya se encuentra en el ambiente destino
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        [HttpPost, Autoriza("nuevoRol")]
        public JsonResult PrevGuardarRol(RolDTO rol)
        {
            string mjs = rolService.PrevGuardarRol(rol);
            return new JsonResult { Data = new { mensaje = mjs, tipoMensaje = "info" } };
        }

        /// <summary>
        /// Metodo que llama al servicio para guardar un rol
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        [Autoriza("nuevoRol"), HttpPost]
        public JsonResult GuardarRol(RolDTO rol)
        {
            rolService.GuardarRol(rol);
            return new JsonResult { Data = new { mensaje = "El Rol ha sido guardado satisfactoriamente.", tipoMensaje = "success" } };
        }

        /// <summary>
        /// Metodo que nos lleva al form para editar un rol
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Autoriza("editarRol")]
        public ActionResult EditarRol(long id)
        {
            RolDTO rol = rolService.GetRolEditar(id);
            ViewBag.Estructuras = new SelectList(rolService.GetComboEstructuras(true), "Codigo", "Descripcion");
            ViewBag.TipoNodo = new SelectList(rolService.GetTiposNodo(), "Codigo", "Descripcion");
            ViewBag.Actividades = rolService.GetComboActividades(rol.EstructuraFuncional);
            return View(rol);
        }

        /// <summary>
        /// Metodo que llama al servicio para guardar la edicion de un rol
        /// </summary>
        /// <param name="rol"></param>
        /// <returns></returns>
        [Autoriza("editarRol"), HttpPost]
        public JsonResult GuardarEdicionRol(RolDTO rol)
        {
            rolService.GuardarEdicionRol(rol);
            return new JsonResult { Data = new { mensaje = "El Rol ha sido guardado satisfactoriamente.", tipoMensaje = "success" } };
        }

        /// <summary>
        /// Metodo que llama al servicio para eliminar un rol
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [Autoriza("eliminarRol"), HttpPost]
        public JsonResult EliminarRol(long id, int version)
        {
            rolService.DeleteRol(id, version);
            return new JsonResult { Data = new { mensaje = "El Rol ha sido eliminado satisfactoriamente.", tipoMensaje = "success" } };
        }
        #endregion

        /// <summary>
        /// Metodo que muestra la pantalla de usuarios segun el rol
        /// </summary>
        /// <returns></returns>
        [Autoriza("listarUsuariosRol")]
        public ActionResult UsuariosPorRol()
        {
            ViewBag.Estructuras = new SelectList(rolService.GetComboEstructuras(), "Codigo", "Descripcion");
            ViewBag.Roles = new SelectList(new List<ComboGenerico>(), "Id", "Descripcion");
            return View("ListadoUsuariosRol", new List<RolUsuarioViewModel>());
        }

        /// <summary>
        /// Metodo que muestra la pantalla de actividades segun el rol
        /// </summary>
        /// <returns></returns>
        [Autoriza("listarActividadesRol")]
        public ActionResult ActividadesPorRol()
        {
            ViewBag.Estructuras = new SelectList(rolService.GetComboEstructuras(), "Codigo", "Descripcion");
            ViewBag.Roles = new SelectList(new List<ComboGenerico>(), "Id", "Descripcion");
            return View("ListadoActividadesRol", new List<RolActividadViewModel>());
        }

        /// <summary>
        /// Obtiene los roles al cambiar la estructura
        /// </summary>
        /// <param name="estructura"></param>
        /// <returns></returns>
        public JsonResult GetRolesChangeEstructura(string cod)
        {
            ViewBag.Roles = new SelectList(rolService.GetComboRoles(cod, true), "Id", "Descripcion");
            return Json(ViewBag.Roles);
        }

        /// <summary>
        /// Metodo que devuelve la partial de datatable de usuarios segun rol
        /// </summary>
        /// <param name="rol"></param>
        /// <param name="est"></param>
        /// <returns></returns>
        public ActionResult ListadoUsuariosPorRol(string rol, string est)
        {
            IList<RolUsuarioViewModel> rolesUsuario = rolService.GetRolesUsuario(rol, est);
            return PartialView("Partials/_ResultadoUsuarioRol", rolesUsuario);
        }

        /// <summary>
        /// Metodo que devuelve la partial de datatable de actividades segun rol
        /// </summary>
        /// <param name="rol"></param>
        /// <param name="est"></param>
        /// <returns></returns>
        [Autoriza("listarActividadesRol")]
        public ActionResult ListadoActividadesPorRol(string rol, string est)
        {
            IList<RolActividadViewModel> rolActividades = rolService.GetRolActividades(rol, est);
            return PartialView("Partials/_ResultadoActividadesRol", rolActividades);
        }

        [Autoriza("asignarRolesMasivamente")]
        public ActionResult AsignarRolesMasivamente()
        {
            ViewBag.Estructuras = new SelectList(rolService.GetComboEstructuras(), "Codigo", "Descripcion");
            //ViewBag.Roles = new SelectList(new List<ComboGenerico>(), "Id", "Descripcion");
            //ViewBag.Nodos = new SelectList(new List<ComboGenerico>(), "Id", "Descripcion");
            //ViewBag.Usuarios = new SelectList(new List<UsuarioAutoComplete>());
            //ViewBag.Usuarios = new SelectList(rolService.UsuariosMultiSelect(), "NombreUsuario", "DescripcionAsignacionMasiva");

            return View(new AsignacionMasivaViewModel());
        }

        //public JsonResult GetRolesYNodos(string cod)
        public ActionResult GetRolesYNodos(string cod)
        {
            //SelectList roles = new SelectList(rolService.GetRolesListadoViewModel(cod), "Codigo", "Descripcion");
            //SelectList nodos = new SelectList(rolService.GetComboNodos(cod), "Codigo", "Descripcion");
            //SelectList usuarios = new SelectList(rolService.UsuariosMultiSelect(), "NombreUsuario", "DescripcionAsignacionMasiva");
            //ViewBag.Usuarios = new SelectList(rolService.UsuariosMultiSelect(), "NombreUsuario", "DescripcionAsignacionMasiva");
            //var jsonResult = new JsonResult
            //{
            //    Data = new
            //    {

            //        Roles = roles,
            //        Nodos = nodos,
            //        Usuarios = usuarios
            //    }
            //};
            //jsonResult.MaxJsonLength = int.MaxValue;
            //return jsonResult;

            ViewBag.Roles = rolService.GetRolesListadoViewModel(cod);
            ViewBag.Nodos = rolService.GetComboNodos(cod);
            //ViewBag.Usuarios = rolService.UsuariosMultiSelect();

            return PartialView("Partials/_AsignarRolesMasivamente");           
        }

        public JsonResult GetUsuariosAutocomplete(string query, bool buscarNominados)
        {
            IList<UsuarioAutoComplete> usuarios = rolService.GetUsuariosAutoComplete(buscarNominados, query);

            //return Json(usuarios, JsonRequestBehavior.AllowGet);

            var jsonResult = new JsonResult();
            jsonResult.Data = usuarios;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult; //Json(usuarios, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, Autoriza("asignarRolesMasivamente")]
        public JsonResult GuardarAsignacionRolesMasivo(string Estructura, string Rol, List<string> Nodos, List<string> Usuarios)
        {
            rolService.AsignarRolesMasivamente(Estructura, Rol, Nodos, Usuarios);
            return new JsonResult { Data = new { mensaje = "Los roles fueron asignados exitosamente.", tipoMensaje = "success" } };
        }
        
    }
}