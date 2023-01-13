using ErrorManagerTGP.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGP.Seguridad.BussinessLogic;
using TGP.Seguridad.BussinessLogic.Dto;


namespace TGP.Seguridad.Controllers
{
    public class NovedadController : Controller
    {
        //protected SeguridadApplicationService service = SeguridadApplicationService.Instance;
        protected NovedadApplicationService novedadService = NovedadApplicationService.Instance;

        [Autoriza("listarNovedad")]
        public ActionResult Listado()
        {

            //Agrego Seteo de Variable session de Fecha Para filtro
            if (Session["FechaInicio"] == null)
            {
                Session["FechaInicio"] = DateTime.Today;
            }

            //Get de todas las estructuras
            ViewBag.Estructuras = new SelectList(novedadService.GetComboEstructuras(true), "Codigo", "Descripcion");

            //Lista de roles con opcion todas por defecto no se selecciona ninguna estructura 
            ViewBag.Roles = new SelectList(new List<ComboGenerico>()
            {
                new ComboGenerico()
                {
                    Id = 0,
                    Codigo = "0",
                    Descripcion = "-- TODOS --"
                }
            }, "Codigo", "Descripcion");
            return View(new List<NovedadDTO>());
        }
        /// <summary>
        /// Metodo q obtiene los roles de una estructura para el listado
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        [Autoriza("listarNovedad")]
        public JsonResult GetRolesEstructura(string cod)
        {
            Session["estructuraSelected"] = cod;
            //Obtiene los roles segun la estructura
            return Json(new SelectList(novedadService.GetComboRoles(cod, true), "Codigo", "Descripcion"));
        }

        /// <summary>
        /// Metodo que devuelve las novedades correspondientes a la estructura y rol seleccionados
        /// </summary>
        /// <param name="estructura"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
        public ActionResult GetNovedades(string estructura, string rol, string fechaInicio, string fechaFin )
        {

            return PartialView("Partials/_ResultadoListadoNovedades", novedadService.GetNovedades(estructura, rol, fechaInicio, fechaFin));
        }

        [Autoriza("eliminarNovedad")]
        public JsonResult Eliminar(long id, int version)
        {
            novedadService.EliminarNovedad(id, version);
            return new JsonResult { Data = new { mensaje = "La novedad fue eliminada con exito.", tipo = "success" } };
        }
        /// <summary>
        /// Metodo que nos lleva al form de creacion de una novedad
        /// </summary>
        /// <returns></returns>
        [Autoriza("nuevaNovedad")]
        public ActionResult Nuevo()
        {
            GetCombosFormNovedad();
            return View("NuevaNovedad", new NovedadDTO() { FechaDesde = DateTime.Today });
        }

        /// <summary>
        /// Metodo que nos lleva al form para la edicion de una novedad
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Autoriza("editarNovedad")]
        public ActionResult Editar(long id)
        {
            NovedadDTO novedad = novedadService.GetNovedadEditar(id);
            GetCombosFormNovedad(novedad.EstructuraFuncionalCodigo);
            return View("EditarNovedad", novedad);
        }

        /// <summary>
        /// Metodo que guarda una edicion de novedad. Se pone ValidateInput en false por
        /// el campo descripcion de novedad que viene con tag html
        /// </summary>
        /// <param name="novedad"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false), Autoriza("editarNovedad")]
        public ActionResult GuardarEdicion(NovedadDTO novedad)
        {
            novedadService.GuardarEdicionNovedad(novedad);
            return new JsonResult { Data = new { mensaje = "La novedad fue editada con exito.", tipoMensaje = "success" } };
        }
        /// <summary>
        /// Metodo que guarda una nueva novedad Se pone ValidateInput en false por
        /// el campo descripcion de novedad que viene con tag html
        /// </summary>
        /// <param name="novedad"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false), Autoriza("nuevaNovedad")]
        public ActionResult GuardarNovedad(NovedadDTO novedad)
        {
            novedadService.GuardarNovedad(novedad);
            return new JsonResult { Data = new { mensaje = "La novedad ha sido creada con éxito.", tipoMensaje = "success" } };
        }
        /// <summary>
        /// Metodo que obtiene los combos a utilizar en el form de novedad
        /// </summary>
        /// <param name="codigoEstructura"></param>
        private void GetCombosFormNovedad(string codigoEstructura = null)
        {
            //Obtiene todas las estructuras menos PORTAL, es la estructura definida para novedades publicas
            ViewBag.Estructuras = new SelectList(novedadService.GetComboEstructuras(true, "-- SELECCIONAR --").Where(x => !x.Codigo.Equals("POR")).ToList(), "Codigo", "Descripcion");

            //Si la estructura es distinta de TODAS, es decir 0, traigo los roles y nodos de esa estructura
            if (codigoEstructura != null && codigoEstructura != "0")
            {
                ViewBag.Roles = novedadService.GetComboRoles(codigoEstructura);
                ViewBag.Nodos = novedadService.GetComboNodos(codigoEstructura);
            }
            //sino devuelvo unas listas vacias
            else
            {
                ViewBag.Roles = new List<ComboGenerico>();
                ViewBag.Nodos = new List<ComboGenerico>();
            }

            //Genero 2 elementos para un combo generico de Tipo de novedad
            ComboGenerico tipoAdvetencia = new ComboGenerico()
            {
                Id = 1,
                Codigo = NovedadDTO.TIPOADVERTENCIA,
                Descripcion = NovedadDTO.TIPOADVERTENCIA
            };

            ComboGenerico tipoNotificacion = new ComboGenerico()
            {
                Id = 2,
                Codigo = NovedadDTO.TIPONOTIFICACION,
                Descripcion = NovedadDTO.TIPONOTIFICACION
            };

            ViewBag.TiposNovedades = new SelectList(new List<ComboGenerico>() { tipoAdvetencia, tipoNotificacion }, "Codigo", "Descripcion");
        }

        /// <summary>
        /// Metodo que devuelve los roles y nodos segun la estructura seleccionada en el form de novedad
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        public JsonResult GetRolesNodoPorEstructura(string cod)
        {
            SelectList selectListRoles = new SelectList(novedadService.GetComboRoles(cod), "Codigo", "Descripcion");
            SelectList selectListNodos = new SelectList(novedadService.GetComboNodos(cod), "Codigo", "Descripcion");
            return new JsonResult
            {
                Data = new
                {
                    roles = selectListRoles,
                    nodos = selectListNodos
                }
            };
        }
    }
}