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
    public class UsuarioController : WebSecurityController
    {

        //protected SeguridadApplicationService service = SeguridadApplicationService.Instance;
        protected UsuarioApplicationService usuarioService = UsuarioApplicationService.Instance;
        protected RolApplicationService rolService = RolApplicationService.Instance;

        #region Nominados
        [Autoriza("listarUsuariosNominados")]
        public ActionResult ListadoNominados()
        {
            ViewBag.Title = "Listado de Usuarios Nominados";
            ViewBag.MenuId = "ListadoNominados-Usuario";
            return View("ListadoNominados", new List<ListadoNominadoViewModel>());
        }

        /// <summary>
        /// Paginado de listado de nominados
        /// </summary>
        /// <returns></returns>
        public JsonResult PaginadoNominados(string marcaActivo, string marcaBloqueado, string marcaTipoAuth)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            string search = Request.Form.GetValues("search[value]")[0];
            //Find Order Column
            string sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            string sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            IList<ListadoNominadoViewModel> nominadosViewModel = usuarioService.GetListadoNominados(draw, start, length, search, sortColumn, sortColumnDir, marcaActivo, marcaBloqueado, marcaTipoAuth);

            long records = nominadosViewModel.Count > 0 ? nominadosViewModel.First().RecordsTotal : 0;

            return Json(new { draw = draw, recordsFiltered = records, recordsTotal = records, data = nominadosViewModel }, JsonRequestBehavior.AllowGet);
        }

        [Autoriza("nuevoNominado")]
        public ActionResult NuevoNominado()
        {
            ViewBag.Servicios = new SelectList(usuarioService.GetServicios(), "C_SERVICIO", "XL_SERVICIO");
            ViewBag.Cargos = new SelectList(usuarioService.GetCargos(), "Codigo", "Descripcion");
            ViewBag.TiposAutenticacion = new SelectList(usuarioService.GetTiposAutenticacion(), "Codigo", "Descripcion");
            return View(new NominadoViewModel());
        }

        [Autoriza("nuevoNominado")]
        public JsonResult GuardarNuevoNominado(NominadoViewModel nuevoNominado)
        {
            usuarioService.GuardarNuevoNominado(nuevoNominado);
            return new JsonResult { Data = new { mensaje = "El Usuario ha sido guardado satisfactoriamente.", tipoMensaje = "success" } };
        }

        /// <summary>
        /// Metodo que elimina un usuario nominado
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v">Nhibernate version</param>
        /// <returns></returns>
        [Autoriza("eliminarNominado")]
        public JsonResult EliminarNominado(long id, int v)
        {
            try
            {
                usuarioService.DeleteUsuario(id, UsuarioDTO.USUARIONOMINADO, v);
                return new JsonResult
                {
                    Data = new
                    {
                        Mensaje = "El Usuario Nominado se eliminó Correctamente.",
                        Tipo = "success",
                        Url = Url.Action("ListadoNominados", "Usuario")
                    }
                };

            }
            catch (Exception e)
            {
                if (e.Message.Equals("ADO"))
                {
                    return new JsonResult
                    {
                        Data = new
                        {
                            Mensaje = "El Usuario Nominado se encuentra relacionado a otro elemento. ¿Desea desactivarlo?",
                            Tipo = "error",
                            Url = Url.Action("MarcarUsuarioInactivo", "Usuario", new { id = id, tipo = UsuarioDTO.USUARIONOMINADO })
                        }
                    };
                }
                else
                    throw e;
            }
        }

        [Autoriza("editarNominados")]
        public ActionResult EditarNominado(long id)
        {
            ViewBag.Cargos = new SelectList(usuarioService.GetCargos(), "Codigo", "Descripcion");
            ViewBag.TiposUsuario = new SelectList(usuarioService.GetTiposUsuario(), "Codigo", "Descripcion");
            NominadoViewModel nominado = usuarioService.GetNominadoDetalle(id);

            #region Usuario con Autenticacion SIGAF
            ViewBag.UsuarioSigafBloqueado = false;
            ViewBag.UsuarioSigafSiDadoBaja = false;
            if (nominado.TipoAutenticacion.Codigo == UsuarioDTO.SIGAF)//Si es del tipo SIGAF verifico su estado Bloqueado o dado de baja
            {
                var usuarioSigaf = usuarioService.GetUsuariosSIGAF(nominado.NombreUsuario);
                if (usuarioSigaf.SiBloqueado == true)
                    ViewBag.UsuarioSigafBloqueado = true;
                else
                    ViewBag.UsuarioSigafBloqueado = false;
                if (usuarioSigaf.FechaBaja != null)
                    ViewBag.UsuarioSigafSiDadoBaja = true;
                else
                    ViewBag.UsuarioSigafSiDadoBaja = false;
            }
            #endregion

            ViewBag.Servicios = usuarioService.GetServicios();
            return View("EditarNominado", nominado);
        }

        /// <summary>
        /// Metodo de edicion de un usuario nominado a aprtir del view model
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [Autoriza("editarNominados"), HttpPost]
        public JsonResult GuardarEdicionNominado(NominadoViewModel usuario)
        {

            usuarioService.EditarNominado(usuario);
            return new JsonResult { Data = new { mensaje = "El Usuario ha sido modificado satisfactoriamente.", tipoMensaje = "success" } };
        }

        /// <summary>
        /// Este metodo se desprende de usuario dashboard, para pasarlos por 2 filtros distintos de autorizacion
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [Autoriza("editarNominados")]
        public ActionResult UsuarioDashboardNominado(UsuarioDashboardViewModel usuario)
        {
            ViewBag.MenuId = "ListadoNominados-Usuario";
            return View("UsuarioDashboard", usuario);
        }

        public ActionResult CargarUsuarioSIGAF(string cUser)
        {
            NominadoViewModel nuevoNominadoSIGAF = usuarioService.CargarUsuarioSIGAF(cUser);
            ViewBag.Servicios = new SelectList(usuarioService.GetServicios(), "C_SERVICIO", "XL_SERVICIO");
            ViewBag.Cargos = new SelectList(usuarioService.GetCargos(), "Codigo", "Descripcion");
            ViewBag.TiposAutenticacion = new SelectList(usuarioService.GetTiposAutenticacion(), "Codigo", "Descripcion");
            return View("NuevoNominado", nuevoNominadoSIGAF);
        }

        /// <summary>
        /// Metodo que devuelve un usuario sigaf para el autocomplete
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public JsonResult GetUsuariosSIGAF(string query)
        {
            IList<UsuarioSIGAFAutocomplete> result = usuarioService.GetUsuariosSIGAFAutocomplete(query);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Metodo que devuelve un usuario sigaf para el autocomplete
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public JsonResult ValidarUsuarioSIGAF(string nombreUsuarioSigaf)
        {
            try
            {
                var result = usuarioService.GetUsuariosSIGAF(nombreUsuarioSigaf);
                if (result.FechaBaja != null)
                    return new JsonResult { Data = new { mensaje = "El usuario se encuentra dado de baja en SIGAF, no se puede desbloquear en el Portal", tipoMensaje = "ERROR" } };
                if (result.SiBloqueado == true)
                    return new JsonResult { Data = new { mensaje = "El usuario podra ser reactivado pero encuentra bloqueado en SIGAF", tipoMensaje = "ATENCION" } };
                return new JsonResult { Data = new { tipoMensaje = "OK" } };
            }
            catch (Exception eSigaf)
            {
                return new JsonResult { Data = new { mensaje = eSigaf.Message, tipoMensaje = "ERROR" } };
            }

        }

        /// <summary>
        /// Metodo que devuelve un usuario sigaf para el autocomplete
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public JsonResult DesactivarUsuario(string id)
        {
            try
            {
                ViewBag.Cargos = new SelectList(usuarioService.GetCargos(), "Codigo", "Descripcion");
                ViewBag.TiposUsuario = new SelectList(usuarioService.GetTiposUsuario(), "Codigo", "Descripcion");
                NominadoViewModel nominado = usuarioService.GetNominadoDetalle(Convert.ToInt64(id));
                nominado.SiActivo = false;
                usuarioService.DesactivarUsuario(nominado);
                return new JsonResult { Data = new { tipoMensaje = "OK" } };
            }
            catch (Exception eSigaf)
            {
                return new JsonResult { Data = new { mensaje = eSigaf.Message, tipoMensaje = "ERROR" } };
            }

        }

        #endregion

        #region Acreedor

        /// <summary>
        /// Paginado de listado de acreedores
        /// </summary>
        /// <returns></returns>
        public JsonResult PaginadoAcreedores()
        {
            //PruebaPaginado();
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            string search = Request.Form.GetValues("search[value]")[0];
            //Find Order Column
            string sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            string sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            IList<ListadoAcreedoresViewModel> acreedoresViewModel = usuarioService.GetListadoAcreedores(draw, start, length, search, sortColumn, sortColumnDir);

            long records = acreedoresViewModel.Count > 0 ? acreedoresViewModel.First().RecordsTotal : 0;

            return Json(new { draw = draw, recordsFiltered = records, recordsTotal = records, data = acreedoresViewModel }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Metodo que nos lleva al listado de acreedores
        /// </summary>
        /// <returns></returns>
        [Autoriza("listarUsuariosAcreedores")]
        public ActionResult ListadoAcreedores()
        {
            ViewBag.Title = "Listado de Acreedores";
            ViewBag.MenuId = "ListadoAcreedores-Usuario";
            return View("ListadoAcreedores", new List<ListadoAcreedoresViewModel>());
        }

        /// <summary>
        /// Metodo que nos lleva a la pantalla para la creacion de un nuevo acreedor
        /// </summary>
        /// <returns></returns>
        [Autoriza("nuevoAcreedor")]
        public ActionResult NuevoAcreedor()
        {
            ViewBag.TiposAutenticacion = new SelectList(usuarioService.GetTiposAutenticacion(), "Codigo", "Descripcion");
            return View(new AcreedorViewModel());
        }

        /// <summary>
        /// Metodo que elimina un usuario acreedor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v">Nhibernate Version</param>
        /// <returns></returns>
        [Autoriza("eliminarAcreedor")]
        public JsonResult EliminarAcreedor(long id, int v)
        {
            try
            {
                usuarioService.DeleteUsuario(id, UsuarioDTO.USUARIOACREEDOR, v);
                return new JsonResult
                {
                    Data = new
                    {
                        Mensaje = "El Usuario Acreedor se eliminó Correctamente.",
                        Tipo = "success",
                        Url = Url.Action("ListadoAcreedores", "Usuario")
                    }
                };

            }
            catch (Exception e)
            {
                if (e.Message.Equals("ADO"))
                {
                    return new JsonResult
                    {
                        Data = new
                        {
                            Mensaje = "El Usuario Acreedor se encuentra relacionado a otro elemento. ¿Desea desactivarlo?",
                            Tipo = "error",
                            Url = Url.Action("MarcarUsuarioInactivo", "Usuario", new { id = id, tipo = UsuarioDTO.USUARIONOMINADO })
                        }
                    };
                }
                else
                    throw e;
            }
        }

        /// <summary>
        /// Metodo que guarda el nuevo acreedor
        /// </summary>
        /// <param name="nuevoAcreedor"></param>
        /// <returns></returns>
        [Autoriza("nuevoAcreedor")]
        public JsonResult GuardarNuevoAcreedor(AcreedorViewModel nuevoAcreedor)
        {
            usuarioService.GuardarNuevoAcreedor(nuevoAcreedor);
            return new JsonResult { Data = new { mensaje = "El Usuario ha sido guardado satisfactoriamente.", tipoMensaje = "success" } };
        }

        /// <summary>
        /// Metodo de edicion de un usuario acreedor a partir del view model
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [Autoriza("editarAcreedor"), HttpPost]
        public JsonResult GuardarEdicionAcreedor(AcreedorViewModel usuario)
        {
            usuarioService.EditarAcreedor(usuario);
            return new JsonResult { Data = new { mensaje = "El Usuario ha sido modificado satisfactoriamente.", tipoMensaje = "success" } };
        }

        /// <summary>
        /// Metodo que nos lleva a la pantalla de edicion de acreedor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Autoriza("editarAcreedor")]
        public ActionResult EditarAcreedor(long id)
        {
            AcreedorViewModel acreedor = usuarioService.GetAcreedorDetalle(id);
            return View("EditarAcreedor", acreedor);
        }

        /// <summary>
        /// Este metodo se desprende de usuario dashboard, para pasarlos por 2 filtros distintos de autorizacion
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [Autoriza("editarAcreedor")]
        public ActionResult UsuarioDashboardAcreedor(UsuarioDashboardViewModel usuario)
        {
            ViewBag.MenuId = "ListadoAcreedores-Usuario";
            return View("UsuarioDashboard", usuario);
        }
        #endregion

        #region Permisos
        /// <summary>
        /// Metodo que carga los permisos del usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Autoriza("editarPermisos")]
        public ActionResult Permisos(long id, string tipoUsuario)
        {
            IList<PermisosViewModel> permisos = usuarioService.GetRolesNodoUsuario(id);
            ViewBag.NombreUsuario = usuarioService.GetNombreUsuario(id);
            ViewBag.IdUsuario = id;
            ViewBag.TipoUsuario = tipoUsuario;
            //ViewBag.Estructuras = new SelectList(usuarioService.GetComboEstructuras(true), "Codigo", "Descripcion");
            ViewBag.MenuId = (tipoUsuario.Equals("A")) ? "ListadoAcreedores-Usuario" : "ListadoNominados-Usuario";
            return View(permisos);
        }


        /// <summary>
        /// Metodo para el refresh de permisos segun la estructura seleccionada
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="codigoEstructura"></param>
        /// <returns></returns>
        public ActionResult ChangeEstructuraFilter(string idUsuario, string codigoEstructura, string tipoUsuario)
        {
            @ViewBag.tipoUsuario = tipoUsuario;
            IList<PermisosViewModel> permisos;
            if (codigoEstructura != null && !codigoEstructura.Equals("0"))
            {
                permisos = usuarioService.GetRolesNodoUsuario(long.Parse(idUsuario), codigoEstructura);
            }
            else
            {
                permisos = usuarioService.GetRolesNodoUsuario(long.Parse(idUsuario));
            }
            Session["estructuraSelected"] = codigoEstructura;
            return PartialView("Partials/_ListadoPermisosUsuario", permisos);
        }


        /// <summary>
        /// Metodo q desasocia un rol de un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="idRol"></param>
        /// <returns></returns>
        [Autoriza("editarPermisos")]
        public JsonResult DesasociarRolUsuario(long idUsuario, long idRol)
        {
            try
            {
                usuarioService.DesasociarRolUsuario(idUsuario, idRol);
                return new JsonResult { Data = new { } };
            }
            catch (Exception e)
            {
                return new JsonResult { Data = new { ErrorMsj = e.Message } };
            }
        }

        /// <summary>
        /// Metodo que nos lleva a la pantalla de asignacion de rol al usuario
        /// </summary>
        /// <param name="id"></param>
        /// <param name="codigoEstructura"></param>
        /// <param name="tipoUsuario"></param>
        /// <returns></returns>
        [Autoriza("nuevoRolUsuario"), HttpPost]
        public ActionResult NuevoRol(long id, string codigoEstructura, string tipoUsuario)
        {
            ViewBag.TipoUsuario = tipoUsuario;
            GetCombosRolUsuario(codigoEstructura);
            Session["estructuraSelected"] = codigoEstructura;
            return View("NuevoRolUsuario", new RolUsuarioViewModel { IdUsuario = id, Estructura = codigoEstructura });
        }

        /// <summary>
        /// Metodo que nos lleva a la pantalla de copia de rol/es del usuario origen a el/los usuario/s que se seleccionen como destinatarios
        /// </summary>
        /// <param name="id2"></param>
        /// <param name="codigoEstructura2"></param>
        /// <param name="tipoUsuario2"></param>
        /// <returns></returns>
        [Autoriza("copiarRolesMasivamente"), HttpGet]
        public ActionResult CopiarRolesMasivamente(long id2, string codigoEstructura2, string tipoUsuario2)
        {
            ViewBag.IdUsuario = id2;
            ViewBag.TipoUsuario = tipoUsuario2;
            ViewBag.Usuarios = new SelectList(rolService.UsuariosMultiSelect(tipoUsuario2), "NombreUsuario", "DescripcionAsignacionMasiva");
            ViewBag.MenuId = (tipoUsuario2.Equals("A")) ? "ListadoAcreedores-Usuario" : "ListadoNominados-Usuario";
            IList<PermisosViewModel> listaPermisos = new List<PermisosViewModel>();
            IList<string> listaUsuarios = new List<string>();
            IList<string> descripcionEstructura = new List<string>();
            IList<string> codigoEstructura = new List<string>();
            listaPermisos = usuarioService.GetRolesNodoUsuario(id2);

            if (codigoEstructura2 != "0" && codigoEstructura2 != null && codigoEstructura2 != "")
            {
                //Busco la descripcion de la estructura en base al parámetro "codigoEstructura2"
                GetCombosRolUsuario(codigoEstructura2);
                string estructura = EstructuraApplicationService.Instance.GetDescripcionEstructura(codigoEstructura2);
                descripcionEstructura.Add(estructura);
                codigoEstructura.Add(codigoEstructura2);
            }
            else
            {
                //Armo una lista de las descripciones de las estructuras que tiene asignadas el Usuario Origen
                descripcionEstructura = listaPermisos.Select(x => x.DescripcionEstructura).Distinct().ToList();
                codigoEstructura = listaPermisos.Select(x => x.CodigoEstructura).Distinct().ToList();
            }

            IList<ComboGenerico> estructuras = usuarioService.GetComboEstructuras();
            IList<ComboGenerico> estructuras2 = new List<ComboGenerico>();
            foreach (string codigo in codigoEstructura)
            {
                ComboGenerico itemComboEstructura = new ComboGenerico();
                itemComboEstructura = estructuras.Where(x => x.Codigo.Equals(codigo)).SingleOrDefault();
                estructuras2.Add(itemComboEstructura);
            }
            ViewBag.Estructuras = new SelectList(estructuras2.OrderBy(x => x.Descripcion), "Codigo", "Descripcion");

            CopiaMasivaPermisosViewModel copiaMasivaPermisos = new CopiaMasivaPermisosViewModel();
            copiaMasivaPermisos.Id = id2;
            copiaMasivaPermisos.TipoUsuario = tipoUsuario2;
            copiaMasivaPermisos.Usuarios = listaUsuarios;
            copiaMasivaPermisos.Permisos = listaPermisos;
            copiaMasivaPermisos.Estructuras = codigoEstructura; //en la version anterior se asignaba descripcionEstructura;
            copiaMasivaPermisos.CodigosEstructuras = codigoEstructura; //copiaMasivaPermisos.CodigosEstructuras se utilizaba en la version anterior
            return View("CopiarRolesMasivamente", copiaMasivaPermisos);
        }

        /// <summary>
        /// Metodo que ejecuta la copia de rol/es del usuario origen (copiaMasivaPermisos.Permisos) a el/los usuario/s seleccionados como destinatarios (copiaMasivaPermisos.Usuarios)
        /// </summary>
        /// <param name="copiaMasivaPermisos"></param>
        /// <returns></returns>
        [Autoriza("copiarRolesMasivamente"), HttpPost]
        public JsonResult CopiarAsignacionRolesMasivo(CopiaMasivaPermisosViewModel copiaMasivaPermisos)
        {
            string msg = rolService.CopiarRolesMasivamente(copiaMasivaPermisos);
            return new JsonResult { Data = new { mensaje = "Los roles fueron asignados exitosamente. \r\n\n" + msg , tipoMensaje = "success" } };
        }

        private void GetCombosRolUsuario(string codigoEstructura, string rol = "")
        {
            IList<ComboGenerico> estructuras = usuarioService.GetComboEstructuras();
            ViewBag.Estructuras = new SelectList(estructuras, "Codigo", "Descripcion");
            ViewBag.Roles = new SelectList(usuarioService.GetComboRoles(codigoEstructura), "Codigo", "Descripcion");
            if (rol != "")
                ViewBag.NodosFuncionales = usuarioService.GetComboNodos(codigoEstructura, rol);
            Session["estructuraSelected"] = codigoEstructura;
        }

        /// <summary>
        /// Obtiene los roles al cambiar la estructura
        /// </summary>
        /// <param name="estructura"></param>
        /// <returns></returns>
        public JsonResult GetRolesChangeEstructura(string estructura)
        {
            Session["estructuraSelected"] = estructura;
            ViewBag.Roles = new SelectList(usuarioService.GetComboRoles(estructura), "Codigo", "Descripcion");
            return Json(ViewBag.Roles);
        }

        /// <summary>
        /// Obtiene los nodos al cambiar el combo de estructura
        /// </summary>
        /// <param name="estructura"></param>
        /// <returns></returns>
        public JsonResult GetNodosChangeRoles(string estructura)
        {
            Session["estructuraSelected"] = estructura;
            ViewBag.NodosFuncionales = new SelectList(usuarioService.GetComboNodos(estructura), "Codigo", "Descripcion");
            return Json(ViewBag.NodosFuncionales);
        }

        /// <summary>
        /// Metodo que guarad la nueva asociacion rol usuario
        /// </summary>
        /// <param name="rolUsuario"></param>
        /// <returns></returns>
        [Autoriza("nuevoRolUsuario")]
        public JsonResult AsociarRolUsuario(RolUsuarioViewModel rolUsuario)
        {
            string resultado = "success";
            string mensaje = "Los permisos se asignaron correctamente.";
            List<string> resultadoAsociarRol = usuarioService.AsociarRolUsuario(rolUsuario);
            ViewBag.ActividadesEnConflicto = resultadoAsociarRol;
            if (resultadoAsociarRol != null)
            {

                resultado = "error";
                mensaje = "Ocurrió un error al asignar los permisos.";
            }

            return new JsonResult
            {
                Data = new
                {
                    mensaje = mensaje,
                    tipoMensaje = resultado,
                    renderedView = RenderViewToString(this.ControllerContext, "Partials/_ActividadesEnConflicto", resultadoAsociarRol)
                }
            };
            //return Json(RenderViewToString(this.ControllerContext, "NuevoRolUsuario", new RolUsuarioViewModel { IdUsuario = rolUsuario.IdUsuario, Estructura = rolUsuario.Estructura }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Metodo q nos lleva a la pantalla de edicion de rol
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditarRolUsuarioLink(long idUsuario, string rol, string tipoUsuario)
        {
            ViewBag.TipoUsuario = tipoUsuario;
            RolUsuarioViewModel rolUsuario = usuarioService.GetRolUsuarioEditar(idUsuario, rol);
            GetCombosRolUsuario(rolUsuario.Estructura, rol);
            return View("EditarRolUsuario", rolUsuario);
        }

        /// <summary>
        /// Metodo que realiza la edicion de un rol para un usuario
        /// </summary>
        /// <param name="rolUsuario"></param>
        /// <param name="rolViejo">Lo necesitamos para borrar y para redireccion en caso de falla</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditarRolUsuario(RolUsuarioViewModel rolUsuario, string rolViejo, string tipoUsuario)
        {

            List<string> resultadoAsociarRol = usuarioService.EditarRolusuario(rolUsuario, rolViejo);
            if (resultadoAsociarRol != null)
            {
                ViewBag.ActividadesEnConflicto = resultadoAsociarRol;
                //Redireccionamos con el codigo viejo porque si llegamos hasta aca significa q fallo la edicion
                return EditarRolUsuarioLink(rolUsuario.IdUsuario, rolViejo, tipoUsuario);
            }

            return RedirectToAction("Permisos", new { id = rolUsuario.IdUsuario, tipoUsuario });
        }

        public PartialViewResult GetNodosSelecionablesPorUsuarioRol(long idUsuario, string rol)
        {
            RolUsuarioViewModel rolUsuario = usuarioService.GetRolUsuarioEditar(idUsuario, rol);
            GetCombosRolUsuario(rolUsuario.Estructura, rol);
            return PartialView("Partials/_SelectNodoFuncional", rolUsuario);
        }
        #endregion

        #region General
        public ActionResult MarcarUsuarioInactivo(long id, string tipo)
        {
            string ActionSiguiente = usuarioService.MarcarUsuarioInactivo(id, tipo);
            return RedirectToAction(ActionSiguiente, "Usuario");
        }
        /// <summary>
        /// Metodo que recupera el avatar del usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="tipoUsuario"></param>
        /// <returns></returns>
        public FileContentResult GetAvatar(long idUsuario)
        {
            byte[] avatar = usuarioService.GetAvatar(idUsuario);
            if (avatar != null)
            {
                return File(avatar, "image/jpeg");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Metodo para el upload de imagenes para Avatar
        /// </summary>
        /// <param name="imagefile"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImageUpload(HttpPostedFileBase imagefile, long idUsuario, string tipoUsuario)
        {
            string messageError = "";
            //UsuarioDTO usuario = (UsuarioDTO)_brokerServices.GetById(idUsuario, typeof(UsuarioDTO));
            List<string> result = new List<string>();

            if (imagefile == null || imagefile.ContentLength == 0)
            {
                if (TempData["errorImage"] == null)
                {
                    TempData.Add("errorImage", "No ha seleccionado una imagen.");
                }
                return RedirectToAction((tipoUsuario.Equals(UsuarioDTO.USUARIONOMINADO) ? "EditarNominado" : "EditarAcreedor"), "Usuario", new { idUsuario = idUsuario });
            }

            // Check image size(max 5MB)
            if (imagefile.ContentLength > 5000000)
                result.Add("El tamaño de la imagen supera el maximo permitido.");

            // Check image extension
            var extension = Path.GetExtension(imagefile.FileName).Substring(1).ToLower();
            if (!"jpg,png".Contains(extension))
            {
                result.Add(string.Format("'{0}' formato no permitido.", extension));
            }

            if (result.Count > 0)
            {
                //String messageError = "";
                foreach (string msg in result)
                {
                    messageError += msg + " ";
                }
                if (messageError != null && messageError != "")
                    ViewBag.Error = messageError;
            }
            else
            {
                usuarioService.GuardarAvatarNuevo(imagefile, idUsuario, tipoUsuario);
            }
            if (tipoUsuario.Equals(UsuarioDTO.USUARIONOMINADO))
                return EditarNominado(idUsuario);
            else
                return EditarAcreedor(idUsuario);
        }

        /// <summary>
        /// Metodo para el envio de mails
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipo"></param>
        /// <param name="version"></param>
        /// <param name="origen"></param>
        /// <returns></returns>
        public ActionResult MailFT(long id, string tipo, int version, string origen, string tipoAutenticacion)
        {

            try
            {
                string result = usuarioService.MailFT(id, tipo, version, tipoAutenticacion);
                if (result.Equals(string.Empty))
                {
                    return new JsonResult
                    {
                        Data = new
                        {
                            Mensaje = "La contraseña se modificó correctamente",
                            Tipo = "success",
                            Url = Url.Action(origen, "Usuario")
                        }
                    };
                }
                else
                {
                    return new JsonResult
                    {
                        Data = new
                        {
                            Mensaje = "El tipo de usuario no es correcto",
                            Tipo = "error",
                            Url = Url.Action(origen, "Usuario")
                        }
                    };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Este metodo deriva a un dashboad o a otro dependiendo del tipo de usuario para ser filtrado por los permisos
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipoUsuario"></param>
        /// <returns></returns>
        public ActionResult UsuarioDashboard(long id, string tipoUsuario)
        {

            if (tipoUsuario.Equals(UsuarioDTO.USUARIOACREEDOR))
                return UsuarioDashboardAcreedor(usuarioService.GetUsuarioDashboard(id, tipoUsuario));
            else
                return UsuarioDashboardNominado(usuarioService.GetUsuarioDashboard(id, tipoUsuario));
        }

        [HttpPost]
        public ActionResult CambiarClave(UsuarioDashboardViewModel usuario)
        {
            CambioPasswordViewModel cambioClave = new CambioPasswordViewModel()
            {
                Id = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                Version = usuario.Version,
                CodigoTipoAutenticacion = usuario.CodigoTipoAutenticacion,
                CodigoTipoUsuario = usuario.CodigoTipoUsuario
            };
            ViewBag.TiposAutenticacion = new SelectList(usuarioService.GetTiposAutenticacion(), "Codigo", "Descripcion");
            return View("CambiarClave", cambioClave);
        }


        /// <summary>
        /// Metodo que envia a guardar la nueva clave luego de las validaciones
        /// </summary>
        /// <param name="newPass"></param>
        /// <returns></returns>
        [Autoriza("cambiarContrasena"), HttpPost]
        public JsonResult GuardarNuevaClave(CambioPasswordViewModel newPass)
        {
            if (newPass.NombreUsuario != null && !newPass.NombreUsuario.Equals(string.Empty))
                if (newPass.NuevaContraseña != null && newPass.NuevaContraseña.Equals(newPass.NuevaContraseñaConfirmacion))
                {
                    usuarioService.GuardarNuevaClave(newPass);
                    ViewBag.MessageConfirm = "¡Clave modificada correctamente!";
                    return new JsonResult
                    {
                        Data = new
                        {
                            mensaje = "Clave modificada correctamente!.",
                            tipoMensaje = "success",
                        }
                    };

                }
                else
                {
                    return new JsonResult
                    {
                        Data = new
                        {
                            mensaje = "La contraseña nueva y la confirmacion no coinciden.",
                            tipoMensaje = "error"
                        }
                    };
                }
            else
            {
                return new JsonResult
                {
                    Data = new
                    {
                        mensaje = "El usuario no fue encontrado.",
                        tipoMensaje = "error"
                    }
                };
            }
        }

        /// <summary>
        /// Metodoq ue manda a cambair el tipo de acutenticacion del usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="tipoAutenticacion"></param>
        /// <param name="tipoUsuario"></param>
        //[Autoriza("cambiarContrasena"), HttpPost]
        //public void CambiarTipoAutenticacion(long idUsuario, string tipoAutenticacion, string tipoUsuario)
        //{
        //    usuarioService.CambiarTipoAutenticacion(idUsuario, tipoAutenticacion, tipoUsuario);

        //}

        [Autoriza("cambiarContrasena"), HttpPost]
        public ActionResult CambiarTipoAutenticacion(long idUsuario, string tipoAutenticacion, string tipoUsuario)
        {

            if (tipoAutenticacion == "2")
            {
                try
                {
                    NominadoViewModel nominado = usuarioService.GetNominadoDetalle(idUsuario);
                    try
                    {
                        var usuarioSigaf = usuarioService.GetUsuariosSIGAF(nominado.NombreUsuario);

                    }
                    catch (Exception e)
                    {
                        return new JsonResult { Data = new { mensaje = "El usuario no existe en SIGAF.", tipoMensaje = "warning" } };
                    }
                    UsuarioDashboardViewModel usuarioDBVM = usuarioService.GetUsuarioDashboard(idUsuario, tipoUsuario);
                    usuarioService.CambiarTipoAutenticacion(idUsuario, tipoAutenticacion, tipoUsuario);

                    usuarioDBVM = usuarioService.GetUsuarioDashboard(idUsuario, tipoUsuario);
                    CambioPasswordViewModel cambioClave = new CambioPasswordViewModel()
                    {
                        Id = usuarioDBVM.Id,
                        NombreUsuario = usuarioDBVM.NombreUsuario,
                        Version = usuarioDBVM.Version,
                        CodigoTipoAutenticacion = usuarioDBVM.CodigoTipoAutenticacion,
                        CodigoTipoUsuario = usuarioDBVM.CodigoTipoUsuario
                    };

                    return new JsonResult
                    {
                        Data = new
                        {
                            cambioDeClave = cambioClave,
                            urlPartialView = RenderViewToString(this.ControllerContext, "Partials/_FormCambioClave", cambioClave)
                        }
                    };

                }
                catch (Exception ex)
                {
                    throw ex;
                    //return new JsonResult { Data = new { mensaje = ex.Message, tipoMensaje = "ERROR" } };
                }


            }
            else
            {
                usuarioService.CambiarTipoAutenticacion(idUsuario, tipoAutenticacion, tipoUsuario);
                UsuarioDashboardViewModel usuarioDBVM = usuarioService.GetUsuarioDashboard(idUsuario, tipoUsuario);
                CambioPasswordViewModel cambioClave = new CambioPasswordViewModel()
                {
                    Id = usuarioDBVM.Id,
                    NombreUsuario = usuarioDBVM.NombreUsuario,
                    Version = usuarioDBVM.Version,
                    CodigoTipoAutenticacion = usuarioDBVM.CodigoTipoAutenticacion,
                    CodigoTipoUsuario = usuarioDBVM.CodigoTipoUsuario
                };

                //return PartialView("Partials/_FormCambioClave", cambioClave);

                return new JsonResult
                {
                    Data = new
                    {
                        cambioDeClave = cambioClave,
                        urlPartialView = RenderViewToString(this.ControllerContext, "Partials/_FormCambioClave", cambioClave)
                    }
                };
            }

        }

        /// <summary>
        /// Helper method to render views/partial views to strings.
        /// </summary>
        /// <param name="context">The controller</param>
        /// <param name="viewName">The name of the view belonging to the controller</param>
        /// <param name="model">The model which is to be passed to the view, if needed.</param>
        /// <returns>A view/partial view rendered as a string.</returns>
        public static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }


        #endregion

        #region Consulta de Asignacion de Usuarios
        /// <summary>
        /// Metodo que muestra la pantalla de consulta de asignacion de Usuarios //Reemplazado por el de abajo(con parámetros)
        /// </summary>
        /// <returns></returns>
        //[Autoriza("consultaAsignacionDeUsuarios")]
        //public ActionResult ConsultaAsignacionDeUsuarios()
        //{
        //    ViewBag.Estructuras = new SelectList(usuarioService.GetComboEstructuras(true), "Codigo", "Descripcion");
        //    ComboGenerico opcionTodos = new ComboGenerico() { Id = 0, Codigo = "0", Descripcion = "-- TODOS --" };
        //    ViewBag.Roles = new SelectList(new List<ComboGenerico>() { opcionTodos }, "Codigo", "Descripcion");
        //    return View("ConsultaAsignacionUsuarios");
        //}

        /// <summary>
        /// Metodo que muestra la pantalla de consulta de asignacion de Usuarios navegando el menú Usuarios, como desde Rol/ListadoActividadesRol.cshtml
        /// </summary>
        /// <returns></returns>
        [Autoriza("consultaAsignacionDeUsuarios")]
        public ActionResult ConsultaAsignacionDeUsuarios(string idEstructura = null, string idRol = null)
        {
            ViewBag.Estructuras = new SelectList(usuarioService.GetComboEstructuras(true), "Codigo", "Descripcion");
            ComboGenerico opcionTodos = new ComboGenerico() { Id = 0, Codigo = "0", Descripcion = "-- TODOS --" };
            ViewBag.Roles = new SelectList(new List<ComboGenerico>() { opcionTodos }, "Codigo", "Descripcion");
            ViewBag.IdEstructura = idEstructura;
            ViewBag.IdRol = idRol;
            return View("ConsultaAsignacionUsuarios");
        }

        public ActionResult ResultadosConsultaAsignaciones(string usuario, string codigoEstructura, string rol)
        {
            IList<AsignacioUsuariosViewModel> resultado = usuarioService.GetListConsultaAsignaciones(usuario, codigoEstructura, rol);
            ViewBag.Estructuras = new SelectList(usuarioService.GetComboEstructuras(true), "Codigo", "Descripcion");
            ViewBag.IdEstructura = null;
            ViewBag.IdRol = null;
            return PartialView("Partials/_ListadoConsultaAsignacionUsuario", resultado);
        }

        /// <summary>
        /// Metodo que devuelve usuarios segun la query realizada para el autocomplete
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public JsonResult GetUsuarioByNombreUsuarioAutoComplete(string query)
        {
            IList<UsuarioAutoComplete> usuarios = usuarioService.GetUsuarioByNombreUsuarioAutoComplete(query);
            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Metodo que devuelve usuarios segun la query realizada para el autocomplete
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public JsonResult GetUsuarioByNombreApellidoRazonSocial(string query)
        {
            IList<UsuarioAutoComplete> usuarios = usuarioService.GetUsuarioByNombreApellidoRazonSocial(query);
            return Json(usuarios, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCombosRol(string cod)
        {
            ViewBag.Roles = new SelectList(usuarioService.GetCombosRol(cod), "Codigo", "Descripcion");
            return Json(ViewBag.Roles);
        }

        #endregion

    }
}