using ErrorManagerTGP.Attributes;
using System.Collections.Generic;
using System.Web.Mvc;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.BussinessLogic;

namespace TGP.Seguridad.Controllers
{
    public class AdministradorLocalController : WebSecurityController
    {
        protected AdministradorLocalApplicationService adminLocalService = AdministradorLocalApplicationService.Instance;
        protected UsuarioApplicationService usuarioService = UsuarioApplicationService.Instance;
        protected RolApplicationService rolService = RolApplicationService.Instance;

        [Autoriza("editarAdministradoresLocales")]
        public ActionResult Listado()
        {
            IList<ListadoAdministradoresLocalesViewModel> listadoNominados = adminLocalService.GetListadoNominadosAdministradores();

            ViewBag.Title = "Listado de Administradores Locales";
            ViewBag.MenuId = "Listado-AdministradorLocal";
            ViewBag.Usuarios = new SelectList(usuarioService.GetUsuariosDGA(), "Id", "NombreUsuario");
            return View(listadoNominados);
        }

        [Autoriza("editarAdministradoresLocales")]
        public ActionResult NuevoAdministrador(long usuarioSelect)
        {
            ViewBag.Title = "Nuevo de Administrador Local";
            ViewBag.MenuId = "Listado-AdministradorLocal";
            ViewBag.Nominado = adminLocalService.GetNominadoById(usuarioSelect);
            ViewBag.Estructuras = new SelectList(EstructuraApplicationService.Instance.GetEstructuras(bool.Parse(Session["EsAdmin"].ToString())),"Codigo", "DescripcionEstructura");
            ViewBag.AdminsExistentes = adminLocalService.AdminsLocalesDeUsuario(usuarioSelect);
            ViewBag.Organismo = usuarioService.GetNombreDelOrganismoDelUsuario(usuarioSelect);
            return View();
        }

        [HttpGet]
        [Autoriza("editarAdministradoresLocales")]
        public ActionResult DetalleAdmin(long id)
        {
            ViewBag.Title = "Detalle de Administrador Local";
            ViewBag.MenuId = "Listado-AdministradorLocal";
            ViewBag.Nominado = adminLocalService.GetNominadoById(id);
            ViewBag.Estructuras = new SelectList(EstructuraApplicationService.Instance.GetEstructuras(bool.Parse(Session["EsAdmin"].ToString())), "Codigo", "DescripcionEstructura");
            ViewBag.AdminsExistentes = adminLocalService.AdminsLocalesDeUsuario(id);
            ViewBag.Organismo = usuarioService.GetNombreDelOrganismoDelUsuario(id);
            ViewBag.Historial = adminLocalService.GetHistorialAdministrador(id);
            return View("NuevoAdministrador");
        }

        [HttpPost]
        [Autoriza("editarAdministradoresLocales")]
        public JsonResult GuardarAdminsLocales(long usuario, string estructura, long[] Roles, long[] Nodos)
        {
            if(adminLocalService.GuardarAdminsLocales(usuario, estructura, Roles, Nodos, Session["Usuario"].ToString()))
            {
                return new JsonResult { Data = new { mensaje = "El administrador local se guardó exitosamente.", tipoMensaje = "success" } };
            }
            else
            {
                return new JsonResult { Data = new { mensaje = "Hubo un error al guardar el administrador local.", tipoMensaje = "error" } };
            }
        }

        [HttpPost]
        [Autoriza("editarAdministradoresLocales")]
        public JsonResult GetRolesYNodos(string cod, long idUsuario)
        {
            SelectList roles = new SelectList(adminLocalService.GetRolesAdministrables(cod), "Id", "Descripcion");
            SelectList nodos = new SelectList(rolService.GetComboNodos(cod), "Id", "Descripcion");
            List<long> rolesExistentes = adminLocalService.RolesAdministradosPorUsuarioYCodigoEstructura(idUsuario, cod);
            List<long> nodosExistentes = adminLocalService.NodosAdministradosPorUsuarioYCodigoEstructura(idUsuario, cod);
            return new JsonResult { Data = new { Roles = roles, Nodos = nodos, RolesExistentes = rolesExistentes, NodosExistentes = nodosExistentes } };
        }

        [Autoriza("editarAdministradoresLocales")]
        public PartialViewResult GetAdminsExistentes(long id)
        {
            ViewBag.AdminsExistentes = adminLocalService.AdminsLocalesDeUsuario(id);
            return PartialView("Partials/_AdminsExistentes");
        }

        [HttpPost]
        public int GetCantidadDEAdminsPorRol(long idRol)
        {
            return adminLocalService.GetCantidadDEAdminsPorRol(idRol);
        }
    }
}