using ErrorManagerTGP.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using TGP.Seguridad.BussinessLogic;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.BussinessLogic.Helpers;
using TGP.Seguridad.Models;

namespace TGP.Seguridad.Controllers
{
    public class AuditoriaController : WebSecurityController
    {
        protected AuditoriaApplicationService auditoriaService = AuditoriaApplicationService.Instance;

        [Autoriza("listarAuditoria")]
        public ActionResult ListadoUsuariosPorAplicacion()
        {
            SetRangoFechas();
            IList<ComboGenerico> comboTipoUsuario = new List<ComboGenerico>()
            {
                new ComboGenerico()
                {
                    Codigo = UsuarioDTO.USUARIONOMINADO,
                    Descripcion = "NOMINADO",
                    Id = 0
                },
                new ComboGenerico()
                {
                    Codigo = UsuarioDTO.USUARIOACREEDOR,
                    Descripcion = "ACREEDOR",
                    Id = 1
                },
            };

            ViewBag.TipoUsuario = new SelectList(comboTipoUsuario, "Codigo", "Descripcion");
            ViewBag.Estructuras = new SelectList(auditoriaService.GetComboEstructuras(true), "Codigo", "Descripcion");
            return View("UsuariosPorAplicacion", new List<UsuarioListadoAuditoria>());
        }

        /// <summary>
        /// Metodo que lleva a la vista de detalle de conexiones de un usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="tipo"></param>
        /// <param name="fechaDesde"></param>
        /// <param name="fechaHasta"></param>
        /// <returns></returns>
        public ActionResult DetalleConexionesUsuario(string usuario, string tipo, string fechaDesde, string fechaHasta)
        {
            DetalleConexionesUsuario detalleConexionesUsuario = auditoriaService.GetDetalleConexionesUsuario(usuario, tipo, DateTime.Parse(fechaDesde), DateTime.Parse(fechaHasta));
            return View(detalleConexionesUsuario);
        }

        /// <summary>
        /// Metodo que Obtiene los usuario y sus conexiones en el rango de fecha seleccionado
        /// </summary>
        /// <param name="estructura"></param>
        /// <param name="tipoUsuario"></param>
        /// <param name="fechaDesde"></param>
        /// <param name="fechaHasta"></param>
        /// <returns></returns>
        public ActionResult GetUsuariosPorAplicacion(string estructura, string tipoUsuario, DateTime fechaDesde, DateTime fechaHasta)
        {
            IList<UsuarioListadoAuditoria> usuariosPorAplicacion = auditoriaService.GetUsuariosPorAplicacion(estructura, tipoUsuario, fechaDesde, fechaHasta);
            return PartialView("Partials/_ResultadoUsuariosPorAplicacion", usuariosPorAplicacion);
        }

        /// <summary>
        /// Metodo que lleva a la vista de Informe de conexiones
        /// </summary>
        /// <returns></returns>
        [Autoriza("verConexiones")]
        public ActionResult Conexiones()
        {
            DateTime FD = HelperService.GetDateTodayStartDay().AddDays(-7);
            DateTime FH = HelperService.GetDateTodayEndDay();
            SetRangoFechasGraficos();
            ViewBag.MaxConexiones = 0;

            //Objeto conexiones
            Conexiones conexiones = new Conexiones()
            {
                CantidadAcreedores = auditoriaService.GetCantidadAcreedores(),
                CantidadAcreedoresActivos = auditoriaService.GetCantidadAcreedoresActivos(),
                CantidadNominados = auditoriaService.GetCantidadNominados(),
                CantidadNominadosActivos = auditoriaService.GetCantidadNominadosActivos()
            };
            
            return View(conexiones);
        }


        private void SetRangoFechasGraficos()
        {
            Session["DateToday"] = DateTime.Parse(DateTime.Today.ToShortDateString());
            if (Session["FechaInicioG"] == null)
            {
                Session["FechaInicioG"] = HelperService.GetDateTodayStartDay().AddDays(-7).ToString();
            }
            if (Session["FechaFinG"] == null)
            {
                Session["FechaFinG"] = HelperService.GetDateTodayEndDay().ToString();
            }
            if (Session["FechaInicioGD"] == null)
            {
                Session["FechaInicioGD"] = HelperService.GetDateTodayStartDay().ToString();
            }
        }

        /// <summary>
        /// Metodo que llama al servicio para obtener el grafico de las conexiones por dia
        /// </summary>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        [HttpPost, Autoriza("verConexiones")]
        public ActionResult GraficosEstructurasPorFecha(string fechaInicio, string fechaFin)
        {
            CultureInfo culture = new CultureInfo("es-AR");
            DateTime FD;
            if (fechaInicio != null)
            {
                FD = DateTime.Parse(fechaInicio, culture);
            }
            else
            {
                FD = DateTime.Today;
            }
            DateTime FH;
            if (fechaFin != null)
            {
                FH = DateTime.Parse(fechaFin, culture).AddDays(1).AddSeconds(-1);
            }
            else
            {
                FH = DateTime.Parse(DateTime.Today.ToShortDateString(), culture).AddDays(1).AddSeconds(-1);
            }
            Session["FechaInicioG"] = FD.ToShortDateString();
            Session["FechaFinG"] = FH.ToShortDateString();

            ViewBag.MaxConexiones = 0;
            return PartialView("Partials/_GraficoConexiones", auditoriaService.GetAuditoriaConexionFecha(FD, FH));
        }

        /// <summary>
        /// Metodo que arma el grafico de conexiones por hora en un dia determinado
        /// </summary>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        [HttpPost, Autoriza("verConexiones")]
        public ActionResult GraficoConexionesPorHora(string fechaInicio)
        {
            CultureInfo culture = new CultureInfo("es-AR");
            DateTime FD;
            if (fechaInicio != null)
            {
                FD = DateTime.Parse(fechaInicio, culture);
            }
            else
            {
                FD = DateTime.Today;
            }
            DateTime FH;
            if (fechaInicio != null)
            {
                FH = DateTime.Parse(fechaInicio, culture).AddDays(1).AddSeconds(-1);
            }
            else
            {
                FH = DateTime.Parse(DateTime.Today.ToShortDateString(), culture).AddDays(1).AddSeconds(-1);
            }

            Session["FechaInicioGD"] = FD.ToShortDateString();

            return PartialView("Partials/_GraficoConexionesDiarias", auditoriaService.GetAuditoriaConexionPorHora(FD, FH));
        }

        private void SetRangoFechas()
        {
            Session["DateToday"] = DateTime.Parse(DateTime.Today.ToShortDateString());
            if (Session["FechaInicio"] == null)
            {
                Session["FechaInicio"] = HelperService.GetDateTodayStartDay().AddDays(-7).ToString();

            }
            if (Session["FechaFin"] == null)
            {
                Session["FechaFin"] = HelperService.GetDateTodayEndDay().ToString();
            }

        }

        [Autoriza("monitorAuditoria")]
        public ActionResult Monitor()
        {
            return View();
        }

        /// <summary>
        /// Metodo que muestra el listado de log por aplicacion
        /// </summary>
        /// <returns></returns>
        [Autoriza("monitorLog")]
        public ActionResult ListadoAplicacionLog()
        {
            ViewBag.Estructuras = new SelectList(auditoriaService.GetComboEstructuras(), "Codigo", "Descripcion");
            return View();
        }

        /// <summary>
        /// Metodo que muestra la lista del log en tiempo real usando SignalR
        /// </summary>
        /// <param name="tipoArchivo"></param>
        /// <param name="estructura"></param>
        /// <returns></returns>
        public ActionResult ListadoAplicacionLogTiempoReal(string tipoArchivo, string estructura)
        {
            ViewBag.TipoArchivo = tipoArchivo;
            ViewBag.CodigoEstructuraFuncional = estructura;
            ViewBag.DescripcionEstructuraFuncional = auditoriaService.GetDescripcionEstructura(estructura);
            return View();
        }

        /// <summary>
        /// Metodo que procesa la solicitud de lectura del log y lo muestra en un listado
        /// </summary>
        /// <param name="tipoArchivo"></param>
        /// <param name="estructura"></param>
        /// <param name="cantidadRegistros"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ListadoAplicacionLog(string tipoArchivo, string estructura, string cantidadRegistros)
        {
            //Recupero la estructura funciopnal
            //EstructuraFuncionalDTOLiviano estr = (EstructuraFuncionalDTOLiviano)_brokerServices.GetById(estructura, typeof(EstructuraFuncionalDTOLiviano));
            string filename = string.Empty;
            try
            {
                //Busco el archivo de log a leer por su tipo
                if (tipoArchivo == "ERROR")
                    filename = Helpers.XMLHelper.ObtenerUbicacionArchivoFromXml(estructura, TipoArchivoAuditoriaAplicacion.Error);
                else if (tipoArchivo == "DEBUG")
                    filename = Helpers.XMLHelper.ObtenerUbicacionArchivoFromXml(estructura, TipoArchivoAuditoriaAplicacion.Debug);
                ViewBag.NombreAplicacion = auditoriaService.GetDescripcionEstructura(estructura).ToUpper();
                ViewBag.TipoArchivo = tipoArchivo.ToUpper();
                //Si hay arhivo para leer
                if (filename != null)
                {
                    Stream stream = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    //File.OpenRead(seclogPath1);
                    StreamReader streamReader = new StreamReader(stream);
                    //Realizo la lectura del log
                    var strResult = Helpers.XMLHelper.ObtenerLineasLog(streamReader, Convert.ToInt32(cantidadRegistros));
                    if (strResult != null)
                    {
                        ////Reordeno el log para mostrar arriba los mas nuevos
                        List<AuditoriaAplicacion> logs = new List<AuditoriaAplicacion>();
                        //Por cada registro armo un objecto Auditoria Aplicacion
                        foreach (string s in strResult.Reverse().ToArray())
                        {
                            AuditoriaAplicacion log = new AuditoriaAplicacion();
                            log.Descripcion = s;
                            logs.Add(log);
                        }
                        if (logs.Count > 0)
                            ViewBag.LogAplicacion = logs;
                        else
                        {
                            ViewBag.LogAplicacion = null;
                            ViewBag.MensajeLogAplicacion = "No hay datos para mostrar";
                        }
                    }
                    else
                        ViewBag.LogAplicacion = null;
                }
                else
                {
                    ViewBag.LogAplicacion = null;
                    ViewBag.MensajeLogAplicacion = "No hay datos para mostrar";
                }
            }
            catch (Exception)
            {
                ViewBag.LogAplicacion = null;
                ViewBag.MensajeLogAplicacion = "El archivo <b>" + filename + "</b> no se encuentra, o no puede ser leido";
            }
            //Cargo la partial
            return PartialView("Partials/_ResultadoAuditoriaAplicacion");
        }
    }
}