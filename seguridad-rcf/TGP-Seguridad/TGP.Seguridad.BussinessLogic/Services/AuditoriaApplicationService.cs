
using Mapster;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Mapping;

namespace TGP.Seguridad.BussinessLogic
{
    public class AuditoriaApplicationService : SeguridadApplicationService
    {
        private static AuditoriaApplicationService instance;

        //Singleton
        public new static AuditoriaApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AuditoriaApplicationService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Metodo que devuelve un detalle de conexiones para un usuario particular
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="tipo"></param>
        /// <param name="fechaDesde"></param>
        /// <param name="fechaHasta"></param>
        /// <returns></returns>
        public DetalleConexionesUsuario GetDetalleConexionesUsuario(string usuario, string tipo, DateTime fechaDesde, DateTime fechaHasta)
        {

            Usuario usuarioDetalle = GetUsuarioPorNombreUsuario(usuario, tipo);
            Search searchConexionesUsuario = new Search(typeof(AuditoriaConexion));
            searchConexionesUsuario.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            searchConexionesUsuario.AddExpression(Restrictions.Eq("Usuario.Id", usuarioDetalle.Id));
            searchConexionesUsuario.AddExpression(Restrictions.Ge("FechaConexion", fechaDesde));
            searchConexionesUsuario.AddExpression(Restrictions.Le("FechaConexion", fechaHasta.AddHours(23).AddMinutes(59).AddSeconds(59)));
            IList<AuditoriaDeConexionesDTO> conexionesUsuario = GetByCriteria<AuditoriaConexion>(searchConexionesUsuario).Adapt<List<AuditoriaDeConexionesDTO>>();
            DetalleConexionesUsuario detalleConexiones = new DetalleConexionesUsuario()
            {
                DescripcionUsuario = usuarioDetalle.GetDescripcionUsuario(),
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                NombreUsuario = usuarioDetalle.NombreUsuario,
                Conexiones = conexionesUsuario
            };
            return detalleConexiones;

        }

        /// <summary>
        /// Metodo que devuelve el listado de usuarios y su auditoria segun el rango de fecha seleccionado
        /// </summary>
        /// <param name="estructura"></param>
        /// <param name="tipoUsuario"></param>
        /// <param name="fechaDesde"></param>
        /// <param name="fechaHasta"></param>
        /// <returns></returns>
        public IList<UsuarioListadoAuditoria> GetUsuariosPorAplicacion(string estructura, string tipoUsuario, DateTime fechaDesde, DateTime fechaHasta)
        {
            //Traemos estas propiedades
            string[] propiedades = new string[]
            {
                "Usuario.Id",
                "Usuario.NombreUsuario",
                "Usuario.TipoUsuario.Codigo",
                "Usuario.TipoUsuario.Descripcion",
                "EstructuraFuncionalCodigo",
                "FechaConexion"
            };
            Search searchUsuarios = new Search(typeof(AuditoriaConexion));
            //Si no selecciono TODAS las estructuras
            if(estructura != ComboGenerico.ComboVacio)
                searchUsuarios.AddExpression(Restrictions.Eq("EstructuraFuncionalCodigo", estructura));

            searchUsuarios.AddExpression(Restrictions.Eq("TipoUsuario.Codigo", tipoUsuario));
            searchUsuarios.AddExpression(Restrictions.Ge("FechaConexion", fechaDesde));
            searchUsuarios.AddExpression(Restrictions.Le("FechaConexion", fechaHasta.AddHours(23).AddMinutes(59).AddSeconds(59)));
            IList<AuditoriaConexion> conexionesUsuario = GetByCriteria<AuditoriaConexion>(searchUsuarios, propiedades);
            //Agrupamos cada auditoria de conexion por Usuario
            IList<UsuarioListadoAuditoria> listadoUsuariosAuditoria = conexionesUsuario.GroupBy(x => x.Usuario.NombreUsuario).Select(u => new UsuarioListadoAuditoria()
            {
                IdUsuario = u.First().Usuario.Id,
                Usuario = u.First().Usuario.NombreUsuario,
                UltimaConexion = u.Select(x=>x.FechaConexion).Max(),
                TipoUsuario = u.First().Usuario.TipoUsuario.Descripcion,
                CodigoTipoUsuario = u.First().Usuario.TipoUsuario.Codigo,
                //Agrupamos todas las estructuras y nos quedamos con el codigo
                AplicacionesAccedidas = u.GroupBy(x=> x.EstructuraFuncionalCodigo).Select(x=>x.First().EstructuraFuncionalCodigo).ToList()
            }).ToList();

            return listadoUsuariosAuditoria;

        }

        /// <summary>
        /// Metodo que devuelve la cantidad de nominados activos
        /// </summary>
        /// <returns></returns>
        public long GetCantidadNominadosActivos()
        {
            Search searchCantidadNominadosActivos = new Search(typeof(Nominado));
            searchCantidadNominadosActivos.AddExpression(Expression.Eq("SiActivo", true));
            return GetCountByCriteria<Nominado>(searchCantidadNominadosActivos);
        }

        /// <summary>
        /// Metodo q devuelve la cantidad de acreedores activos
        /// </summary>
        /// <returns></returns>
        public long GetCantidadAcreedoresActivos()
        {
            Search searchCantidadAcreedoresActivos = new Search(typeof(Acreedor));
            searchCantidadAcreedoresActivos.AddExpression(Expression.Eq("SiActivo", true));
            return GetCountByCriteria<Acreedor>(searchCantidadAcreedoresActivos);
        }

        /// <summary>
        /// Metodo que devuelve el total de nominados
        /// </summary>
        /// <returns></returns>
        public long GetCantidadNominados()
        {
            return GetCountByCriteria<Nominado>(new Search(typeof(Nominado)));
        }


        /// <summary>
        /// Metodo que devuelve el total de acreedores
        /// </summary>
        /// <returns></returns>
        public long GetCantidadAcreedores()
        {
            return GetCountByCriteria<Acreedor>(new Search(typeof(Acreedor)));
        }

        /// <summary>
        /// Devuelve un DTO para armar el grafico con la cantidad de conexiones segun el tipo de usuario.
        /// </summary>
        /// <param name="FD"></param>
        /// <param name="FH"></param>
        /// <param name="idEstructura"></param>
        /// <returns>GraficosConexionDTO</returns>
        public GraficosConexionesFecha GetAuditoriaConexionFecha(DateTime FD, DateTime FH)
        {
            GraficosConexionesFecha graficoConexion = new GraficosConexionesFecha();
            //Lista de elementos a graficar en la vista.
            List<ElementoGrafico> listaElementos = new List<ElementoGrafico>();
            listaElementos.AddRange(inicializarListaElementos(FD, FH));
            listaElementos.AddRange(calcularConexiones(UsuarioDTO.USUARIONOMINADO, FD, FH, false));
            listaElementos.AddRange(calcularConexiones(UsuarioDTO.USUARIOACREEDOR, FD, FH, false));

            //Agrupo la Lista por fecha (para cada fecha X, Y nominados, Y' acreedores)
            listaElementos = listaElementos.GroupBy(e => e.Fecha).Select(elem => new ElementoGrafico
            {
                Fecha = elem.Key,
                Nominados = elem.Sum(e => e.Nominados),
                Acreedores = elem.Sum(e => e.Acreedores),
            }).ToList();

            //Recorro la lista de elementos y la agrego al DTO del grafico con el formato requerido.
            foreach (ElementoGrafico elemento in listaElementos)
            {
                if (graficoConexion.Fechas == "" || graficoConexion.Fechas == null)
                {
                    graficoConexion.Fechas += "gt(" + elemento.Fecha.Year + "," + elemento.Fecha.Month + "," + elemento.Fecha.Day + ")";
                    graficoConexion.Acreedores += elemento.Acreedores;
                    graficoConexion.Nominados += elemento.Nominados;

                }
                else
                {
                    graficoConexion.Fechas += ", gt(" + elemento.Fecha.Year + "," + elemento.Fecha.Month + "," + elemento.Fecha.Day + ")";
                    graficoConexion.Acreedores += ", " + elemento.Acreedores;
                    graficoConexion.Nominados += ", " + elemento.Nominados;
                }
            }
            return graficoConexion;
        }

        /// <summary>
        /// Devuelve DTO con el para armar el grafico con la cantidad de conexiones por hora en el día
        /// </summary>
        /// <param name="FD"></param>
        /// <param name="FH"></param>
        /// <returns></returns>
        public GraficoConexionesHora GetAuditoriaConexionPorHora(DateTime FD, DateTime FH)
        {
            GraficoConexionesHora graficoConexion = new GraficoConexionesHora();
            //ViewBag.MaxConexiones = 0;
            //Lista de elementos a graficar en la vista.
            List<ElementoGrafico> listaElementos = new List<ElementoGrafico>();
            listaElementos.AddRange(inicializarListaElementos(FH));
            listaElementos.AddRange(calcularConexiones(UsuarioDTO.USUARIONOMINADO, FD, FH, true));
            listaElementos.AddRange(calcularConexiones(UsuarioDTO.USUARIOACREEDOR, FD, FH, true));

            //Agrupo la Lista por fecha (para cada fecha X, Y nominados, Y' acreedores)
            listaElementos = listaElementos.GroupBy(e => new { e.Hora, e.Fecha.Date }).Select(elem => new ElementoGrafico
            {
                Fecha = elem.Key.Date,
                Hora = elem.Key.Hora,
                Nominados = elem.Sum(e => e.Nominados),
                Acreedores = elem.Sum(e => e.Acreedores),
            }).ToList();

            //Recorro la lista de elementos y la agrego al DTO del grafico con el formato requerido por el c3.js
            foreach (ElementoGrafico elemento in listaElementos)
            {
                if (graficoConexion.Horas == "" || graficoConexion.Horas == null)
                {
                    graficoConexion.Horas += elemento.Hora;
                    graficoConexion.Acreedores += elemento.Acreedores;
                    graficoConexion.Nominados += elemento.Nominados;
                }
                else
                {
                    graficoConexion.Horas += ", " + elemento.Hora;
                    graficoConexion.Acreedores += ", " + elemento.Acreedores;
                    graficoConexion.Nominados += ", " + elemento.Nominados;
                }
            }
            return graficoConexion;
        }

        /// <summary>
        /// Inicializa en 0 los elementos del grafico de conexiones por dia, ya que si no tiene conexiones tiene que mostrar 0
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        private List<ElementoGrafico> inicializarListaElementos(DateTime fechaDesde, DateTime fechaHasta)
        {
            int cantFechas = (fechaHasta - fechaDesde).Days;
            List<ElementoGrafico> elementos = new List<ElementoGrafico>();
            for (int i = 0; i <= cantFechas; i++)
            {
                ElementoGrafico elemento = new ElementoGrafico();
                elemento.Acreedores = 0;
                elemento.Nominados = 0;
                //elemento.Hora = i;
                elemento.Fecha = fechaDesde.AddDays(i);
                elementos.Add(elemento);
            }
            return elementos;
        }

        /// <summary>
        /// Inicializa en 0 los elementos del grafico de conexiones por hora, ya que si no tiene conexiones tiene que mostrar 0
        /// </summary>
        /// <param name="fecha"></param>
        /// <returns></returns>
        private List<ElementoGrafico> inicializarListaElementos(DateTime fecha)
        {
            List<ElementoGrafico> elementos = new List<ElementoGrafico>();
            for (int i = 0; i < 24; i++)
            {
                ElementoGrafico elemento = new ElementoGrafico();
                elemento.Acreedores = 0;
                elemento.Nominados = 0;
                elemento.Hora = i;
                elemento.Fecha = fecha;
                elementos.Add(elemento);
            }
            return elementos;
        }

        /// <summary>
        /// Método que carga la lista de elementos de Acreedores y Nominados para el gráfico
        /// </summary>
        /// <param name="tipoUsuario"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="idEstructura"></param>
        /// <returns>List<ElementoGraficoDTO></returns>
        private List<ElementoGrafico> calcularConexiones(String tipoUsuario, DateTime fechaInicio, DateTime fechaFin, bool siGraficoHoras)
        {
            List<ElementoGrafico> elementos = new List<ElementoGrafico>();
            List<AuditoriaGroupByDate> datosAuditoria = new List<AuditoriaGroupByDate>();

            //Muestra las conexiones solamente para la estructura funcional PORTAL
            EstructuraFuncional est = GetEstructuraPorCodigo("POR");

            //Si el rango de fechas es para un dia especifico armo el grafico por Horas
            if (siGraficoHoras)
            {
                datosAuditoria = GetDatosAuditoriaPorEstructuraPorHora(fechaInicio, fechaFin, tipoUsuario, est.Id);
            }
            else
            {
                datosAuditoria = GetDatosAuditoriaPorEstructura(fechaInicio, fechaFin, tipoUsuario, est.Id);
            }
            //long maxGral = ViewBag.MaxConexiones;
            //long maxLocal = datosAuditoria.Count > 0 ? datosAuditoria.Max(a => a.Conexiones) : 0;
            //if (maxGral < maxLocal)
            //{
            //    ViewBag.MaxConexiones = maxLocal;
            //}

            foreach (AuditoriaGroupByDate auditoria in datosAuditoria)
            {
                ElementoGrafico elemento = new ElementoGrafico();
                elemento.Fecha = auditoria.Fecha;
                elemento.Hora = auditoria.Hora;
                if (tipoUsuario == UsuarioDTO.USUARIONOMINADO)
                {
                    elemento.Nominados = auditoria.Conexiones;
                    elemento.Acreedores = 0;
                }
                else if (tipoUsuario == UsuarioDTO.USUARIOACREEDOR)
                {
                    elemento.Nominados = 0;
                    elemento.Acreedores = auditoria.Conexiones;
                }
                elementos.Add(elemento);
            }
            return elementos;
        }

        /// <summary>
        /// Metodo que obtiene los datos de auditoria y los agrupa por fecha y estructura
        /// </summary>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="tipoUsuario"></param>
        /// <param name="idEstructura"></param>
        /// <returns></returns>
        private List<AuditoriaGroupByDate> GetDatosAuditoriaPorEstructuraPorHora(DateTime fechaInicio, DateTime fechaFin, string tipoUsuario, long idEstructura)
        {
            ISession session = Session();
            EstructuraFuncional est = session.QueryOver<EstructuraFuncional>().Where(e => e.Id == idEstructura).SingleOrDefault();
            return session.QueryOver<AuditoriaConexion>().Where(a => a.FechaConexion >= fechaInicio && a.FechaConexion <= fechaFin && a.EstructuraFuncionalCodigo == est.Codigo)
                   .JoinQueryOver<Usuario>(a => a.Usuario).JoinQueryOver<TipoUsuario>(u => u.TipoUsuario).Where(t => t.Codigo == tipoUsuario).List()
                   .GroupBy(a => new { a.FechaConexion, a.FechaConexion.Hour }).Select(g => new AuditoriaGroupByDate
                   {
                       Fecha = g.Key.FechaConexion,
                       Hora = g.Key.Hour,
                       Conexiones = g.Count()
                   }).OrderByDescending(g => g.Fecha).ThenByDescending(g => g.Hora).ToList();
        }


        /// <summary>
        /// Metodo que obtiene los datos de auditoria segun la estructura
        /// </summary>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="tipoUsuario"></param>
        /// <param name="idEstructura"></param>
        /// <returns></returns>
        List<AuditoriaGroupByDate> GetDatosAuditoriaPorEstructura(DateTime fechaInicio, DateTime fechaFin, string tipoUsuario, long idEstructura)
        {
            ISession session = Session();
            EstructuraFuncional est = session.QueryOver<EstructuraFuncional>().Where(e => e.Id == idEstructura).SingleOrDefault();
            return session.QueryOver<AuditoriaConexion>().Where(a => a.FechaConexion >= fechaInicio && a.FechaConexion <= fechaFin && a.EstructuraFuncionalCodigo == est.Codigo)
                   .JoinQueryOver<Usuario>(a => a.Usuario).JoinQueryOver<TipoUsuario>(u => u.TipoUsuario).Where(t => t.Codigo == tipoUsuario).List()
                   .GroupBy(a => a.FechaConexion.Date).Select(g => new AuditoriaGroupByDate
                   {
                       Fecha = g.Key,
                       Conexiones = g.Count()
                   }).OrderByDescending(g => g.Fecha).ToList();
        }
    }

}
