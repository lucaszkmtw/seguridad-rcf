using ErrorManagerTGP.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TGP.Seguridad.BussinessLogic;
using TGP.Seguridad.BussinessLogic.Dto;

namespace TGP.Seguridad.Controllers
{
    public class ComparadorController : WebSecurityController
    {
        //protected SeguridadApplicationService service = SeguridadApplicationService.Instance;
        protected ComparadorApplicationService comparadorService = ComparadorApplicationService.Instance;


        /// <summary>
        /// Metodo que muestra la vista inicial de la comparativa entre roles de 2 ambientes
        /// </summary>
        /// <returns></returns>
        [Autoriza("compararRoles")]
        public ActionResult CompararRoles()
        {
            //Creamos un nuevo comparador con especificaciones de Roles
            ComparadorGenerico comparador = new ComparadorGenerico()
            {
                TipoComparador = ComparadorGenerico.ROL,
                ElementosOrigen = new List<ElementoComparar>(),
                ElementosDestino = new List<ElementoComparar>(),
                OpcionActivar = ComparadorGenerico.ACTIVAROPCIONROL,
                TextoCopiarDestino = "¿Desea mover el Rol ",
                TextoCopiarDestinoMasivo = "¿Desea mover el(los) Roles seleccionados?",
                TextoEliminarDestino = "¿Desea eliminar el Rol "
            };

            //Todas las estructuras posibles
            ViewBag.Estructuras = new SelectList(comparadorService.GetComboEstructurasComparar(), "Codigo", "Descripcion");

            //Titulo de la pagina
            ViewBag.Titulo = "Comparador de Roles";
            return View("CompararEntidades", comparador);
        }

        /// <summary>
        /// Metodo que muestra la vista inicial de la comparativa entre Estructuras de 2 ambientes
        /// </summary>
        /// <returns></returns>
        [Autoriza("compararEstructurasFuncionales")]
        public ActionResult CompararEstructuras()
        {
            //Creamos un nuevo comparador con especificaciones de Estructuras
            ComparadorGenerico comparador = new ComparadorGenerico()
            {
                TipoComparador = ComparadorGenerico.ESTRUCTURA,
                ElementosOrigen = new List<ElementoComparar>(),
                ElementosDestino = new List<ElementoComparar>(),
                OpcionActivar = ComparadorGenerico.ACTIVAROPCIONESTRUCTURAS,
                TextoCopiarDestino = "¿Desea mover la Estructura ",
                TextoCopiarDestinoMasivo = "¿Desea mover la(las) Estructuras seleccionados?",
                TextoEliminarDestino = "¿Desea eliminar la Estructura "
            };

            //Todas las estructuras posibles
            ViewBag.Estructuras = new SelectList(new List<ComboGenerico>(), "Codigo", "Descripcion");

            //Titulo de la pagina
            ViewBag.Titulo = "Comparador de Estructuras";
            return View("CompararEntidades", comparador);
        }


        /// <summary>
        /// Metodo que muestra la vista inicial de la comparativa entre roles de 2 ambientes
        /// </summary>
        /// <returns></returns>
        [Autoriza("compararActividades")]
        public ActionResult CompararActividades()
        {
            //Creamos un nuevo comparador con especificaciones de Roles
            ComparadorGenerico comparador = new ComparadorGenerico()
            {
                TipoComparador = ComparadorGenerico.ACTIVIDAD,
                ElementosOrigen = new List<ElementoComparar>(),
                ElementosDestino = new List<ElementoComparar>(),
                OpcionActivar = ComparadorGenerico.ACTIVAROPCIONACTIVIDADES,
                TextoCopiarDestino = "¿Desea mover la Actividad ",
                TextoCopiarDestinoMasivo = "¿Desea mover la(las) Actividades seleccionados?",
                TextoEliminarDestino = "¿Desea eliminar la Actividad "
            };

            //Todas las estructuras posibles
            ViewBag.Estructuras = new SelectList(comparadorService.GetComboEstructurasComparar(), "Codigo", "Descripcion");

            //Titulo de la pagina
            ViewBag.Titulo = "Comparador de Actividades";
            return View("CompararEntidades", comparador);
        }

        /// <summary>
        /// Metodo que muestra la vista inicial de la comparativa entre nodos de 2 ambientes
        /// </summary>
        /// <returns></returns>
        [Autoriza("compararNodosFuncionales")]
        public ActionResult CompararNodos()
        {
            //Creamos un nuevo comparador con especificaciones de Roles
            ComparadorGenerico comparador = new ComparadorGenerico()
            {
                TipoComparador = ComparadorGenerico.NODO,
                ElementosOrigen = new List<ElementoComparar>(),
                ElementosDestino = new List<ElementoComparar>(),
                OpcionActivar = ComparadorGenerico.ACTIVAROPCIONNODOS,
                TextoCopiarDestino = "¿Desea mover el Nodo Funcional ",
                TextoCopiarDestinoMasivo = "¿Desea mover el(los) Nodos seleccionados?",
                TextoEliminarDestino = "¿Desea eliminar el Nodo Funcional "
            };

            //Todas las estructuras posibles
            ViewBag.Estructuras = new SelectList(comparadorService.GetComboEstructurasComparar(), "Codigo", "Descripcion");

            //Titulo de la pagina
            ViewBag.Titulo = "Comparador de Nodos Funcionales";
            return View("CompararEntidades", comparador);
        }

        [Autoriza("compararMenuOpcion")]
        public ActionResult CompararMenues()
        {
            //Creamos un nuevo comparador con especificaciones de Roles
            ComparadorGenerico comparador = new ComparadorGenerico()
            {
                TipoComparador = ComparadorGenerico.MENUOPCION,
                ElementosOrigen = new List<ElementoComparar>(),
                ElementosDestino = new List<ElementoComparar>(),
                OpcionActivar = ComparadorGenerico.ACTIVAROPCIONMENUES,
                TextoCopiarDestino = "¿Desea mover el Menú ",
                TextoCopiarDestinoMasivo = "¿Desea mover el(los) Menues seleccionados?",
                TextoEliminarDestino = "¿Desea eliminar el Menú "
            };

            //Todas las estructuras posibles
            ViewBag.Estructuras = new SelectList(comparadorService.GetComboEstructurasComparar(), "Codigo", "Descripcion");

            //Titulo de la pagina
            ViewBag.Titulo = "Comparador de Menues";
            return View("CompararEntidades", comparador);
        }

        /// <summary>
        /// Este metodo devuelve el comparador con los elementos origen y destinos cargados
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public ActionResult GetEntidadesOrigenDestino(ComparadorGenerico comp)
        {
            Session["estructuraSelected"] = comp.EstructuraSeleccionada;
            comp.ElementosDestino = comparadorService.GetElementosDestino(comp.TipoComparador, comp.EstructuraSeleccionada);
            comp.ElementosOrigen = comparadorService.GetElementosOrigen(comp.TipoComparador, comp.EstructuraSeleccionada);

            //Todos los elementos que se encuentres en el Origen pero no en el Destino se los marca con 'N' por nuevos
            foreach (ElementoComparar elemento in comp.ElementosOrigen.Where(o => comp.ElementosDestino.All(d => d.Id != o.Id)))
            {
                elemento.Estado = 'N';
            }
            //Todos los elementos que se encuentras en el destino pero no en el origen se marcan con 'X' por la posibilidad de eliminarlos
            foreach (ElementoComparar elemento in comp.ElementosDestino.Where(d => comp.ElementosOrigen.All(o => o.Id != d.Id)))
            {
                elemento.Estado = 'X';
            }

            //Por defecto tienen marcado 'M' para sobreescribirlos

            return PartialView("Partials/_ListadoComparacion", comp);
        }


        /// <summary>
        ///  Metodo que invoca al servicio para pasar uno o mas elementos de ambiente
        /// </summary>
        /// <param name="cod">Los codigos de los elementos</param>
        /// <param name="tipoComparador">El tipo de elemento que estamos pasando</param>
        /// <param name="estructura">La estructura a la que pertenece, puede ser nulo, en el caso de que estemos pasando una estructura</param>
        /// <returns></returns>
        public JsonResult CopiarDestino(string[] cod, string tipoComparador, string estructura = null)
        {
            string mensajeSuccess;
            //Si viene mas de un codigo de elemento significa q vamos por el masivo
            if (cod.Length > 1)
            {
                mensajeSuccess = comparadorService.CopiarDestinoMasivo(cod, tipoComparador, estructura);
                return new JsonResult { Data = new { mensaje = mensajeSuccess, tipoMensaje = "success" } };
            }
            else
            {
                mensajeSuccess = comparadorService.CopiarDestino(cod, tipoComparador, estructura);
                return new JsonResult { Data = new { mensaje = mensajeSuccess, tipoMensaje = "success" } };
            }
        }

        /// <summary>
        /// Metodo que elimina una entidad del ambiente destino
        /// </summary>
        /// <param name="id">Id de la entidad en destino</param>
        /// <param name="version">Verison de la entidad en destino</param>
        /// <param name="tipoComparador"></param>
        /// <returns></returns>
        public JsonResult EliminarDestino(long id, int version, string tipoComparador)
        {

            string mensajeSuccess = comparadorService.EliminarDestino(id, version, tipoComparador);
            return new JsonResult { Data = new { mensaje = mensajeSuccess, tipoMensaje = "success" } };
        }
    }
}