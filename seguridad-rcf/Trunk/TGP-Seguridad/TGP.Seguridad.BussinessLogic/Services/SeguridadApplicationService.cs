
using Mapster;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Linq;
using TGP.Seguridad.BussinessLogic.Generics;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Mapping;
using System;
using TGP.Seguridad.DataAccess;
using System.Configuration;
using System.Net.Http;
using TGP.Seguridad.BussinessLogic.APIRequests;
using Utils;
using Newtonsoft.Json;
using TGP.Seguridad.BussinessLogic.APIResponses;
using TGP.Seguridad.BussinessLogic.Dto.APIRequest;

namespace TGP.Seguridad.BussinessLogic
{
    
    /// <summary>
    /// Aqui se declararan los metodos que invocaran a los servicios de los casos de uso,
    /// los cuales no deberian ser desarrollados en el GenericService.
    /// </summary>
    public class SeguridadApplicationService : GenericService
    {
        private static SeguridadApplicationService instance;
        private static readonly string MetodoGetNovedades = ConfigurationManager.AppSettings["SegApi_Metodo_ObtenerNovedades"];

        //Singleton
        public new static SeguridadApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SeguridadApplicationService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Encapsulamiento interno del repositorio.
        /// </summary>
        protected NHibernateRepository repository = NHibernateRepository.Instance;



        #region General
        public bool ExisteCodigoElemento<T>(string cod, ISession altSession = null) where T: class
        {
            Search search = new Search(typeof(T));
            search.AddExpression(Restrictions.Eq("Codigo", cod));
            int count = GetCountByCriteria<T>(search, altSession);
            return count > 0;
        }

        public bool ExisteDescripcionElemento<T>(string descripcion, ISession altSession = null) where T : class
        {
            Search search = new Search(typeof(T));
            search.AddExpression(Restrictions.Eq("Descripcion", descripcion));
            int count = GetCountByCriteria<T>(search, altSession);
            return count > 0;
        }

        /// <summary>
        /// Metodo que recupera las novedades desde la api de seguridad
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public Tuple<List<NovedadDTO>, List<NovedadDTO>, List<NovedadDTO>, List<NovedadDTO>, List<NovedadDTO>> ObtenerNovedades(string usuario)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    // Call asynchronous network methods in a try/catch block to handle exceptions

                    //Request para el comparar
                    APIRequests.RequestObtenerNovedades request = new APIRequests.RequestObtenerNovedades()
                    {
                        NombreUsuario = usuario,
                        KeyEncrypt = Encriptador.Encriptar(ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString()),
                    };

                    //mando la notificacion a la api de seguridad
                    var response = ValidarResponseObtenerNoveades(client, request);
                    return Tuple.Create(response.NovedadesNoLeidas, response.AdvertenciasTotalesVigentes, response.AdvertenciasNoLeidas, response.NovedadesVigentes, response.NovedadesGuardadas);

                };
            }
            catch (Exception ex)
            {

                throw new Exception("Problemas al recuperar las novedades: " + ex.Message);
            }
        }

        /// <summary>
        /// Metodo que valida al respuesta segun nuestros codigos de respuesta para la api seg
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        private static ResponseObtenerNovedades ValidarResponseObtenerNoveades(HttpClient client, APIRequests.RequestAPI request)
        {
            //Serializamos el request, con esta opcion evitamos las referencias circulares
            string requestJson = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            StringContent content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(ConfigurationManager.AppSettings["UrlApiSegNuevaNovedad"].ToString() + MetodoGetNovedades, content).Result;
            response.EnsureSuccessStatusCode();
            ResponseObtenerNovedades responseApi = JsonConvert.DeserializeObject<ResponseObtenerNovedades>(response.Content.ReadAsStringAsync().Result);
            if (responseApi.Codigo != "200")
                throw new Exception(responseApi.Mensaje);
            else
                return responseApi;
        }

        public bool ExisteCodigoNodo(string cod, string codigoEstructura)
        {
            Search search = new Search(typeof(NodoFuncional));
            search.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            search.AddExpression(Restrictions.Eq("Codigo", cod));
            search.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
            int count = GetCountByCriteria<NodoFuncional>(search);
            return count > 0;
        }

        public IList<ComboGenerico> GetCargos()
        {
            IList<ComboGenerico> cargos = GetAll<NivelJerarquico>().Adapt<IList<ComboGenerico>>();
            return cargos;
        }

        public IList<BServicioDTO> GetServicios()
        {
            IList<BServicioDTO> bServicios = GetAll<BServicio>().Adapt<IList<BServicioDTO>>();
            return bServicios;
        }

        public IList<ComboGenerico> GetTiposUsuario()
        {
            string[] propiedades = new string[]
            {
                "Codigo",
                "Descripcion"
            };

            IList<ComboGenerico> tiposDeUsuario = GetAll<TipoUsuario>().Adapt<IList<ComboGenerico>>().OrderBy(x => x.Descripcion).ToList();
            return tiposDeUsuario;
        }

        public IList<ComboGenerico> GetTiposAutenticacion()
        {
            IList<ComboGenerico> tiposDeUsuario = GetAll<TipoAutenticacion>().Adapt<IList<ComboGenerico>>().OrderBy(x => x.Descripcion).ToList();
            return tiposDeUsuario;
        }

        public IList<ComboGenerico> GetTiposNodo()
        {
            IList<ComboGenerico> tiposNodos = GetAll<TipoNodoFuncional>().Adapt<IList<ComboGenerico>>().OrderBy(x => x.Descripcion).ToList();
            tiposNodos.Add(new ComboGenerico() {
                Codigo = "TODOS",
                Descripcion = "TODOS",  
            });
            return tiposNodos;
        }

        public IList<ComboGenerico> GetComboEstructuras(bool siConOpcionTodos = false, string placaholder = "")
        {
            //string[] propiedades = new string[]
            //{
            //    "Id",
            //    "Codigo",
            //    "DescripcionEstructura"
            //};

            List<ComboGenerico> estructuras = new List<ComboGenerico>();
            if (siConOpcionTodos)
            {
                if (placaholder != "")
                {
                    estructuras.Add(new ComboGenerico()
                    {
                        Id = 0,
                        Codigo = "0",
                        Descripcion = placaholder
                    });
                }
                else
                {
                    estructuras.Add(new ComboGenerico()
                    {
                        Id = 0,
                        Codigo = "0",
                        Descripcion = "-- TODAS --"
                    });
                }

            }

            estructuras.AddRange(GetAll<EstructuraFuncional>().Adapt<IList<ComboGenerico>>().OrderBy(x => x.Descripcion).ToList());
            return estructuras;
        }


        public IList<ComboGenerico> GetComboRoles(string codigoEstructura, bool conOpcionTodos = false)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Descripcion"
            };
            Search searchRoles = new Search(typeof(Rol));
            searchRoles.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchRoles.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
            List<ComboGenerico> roles = new List<ComboGenerico>();
            if (conOpcionTodos)
            {
                roles.Add(new ComboGenerico()
                {
                    Id = 0,
                    Codigo = "0",
                    Descripcion = "-- TODOS --"
                });
            }
            roles.AddRange(GetByCriteria<Rol>(searchRoles, propiedades).Adapt<IList<ComboGenerico>>().OrderBy(x => x.Descripcion).ToList());
            return roles;
        }

        public IList<ComboGenerico> GetComboActividades(string codigoEstructura, bool conOpcionTodos = false)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Descripcion"
            };
            Search searchActividades = new Search(typeof(Actividad));
            searchActividades.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchActividades.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
            List<ComboGenerico> actividades = new List<ComboGenerico>();

            if (conOpcionTodos)
            {
                actividades.Add(new ComboGenerico()
                {
                    Id = 0,
                    Codigo = "0",
                    Descripcion = "-- TODAS --"
                });
            }

            actividades.AddRange(GetByCriteria<Actividad>(searchActividades, propiedades).Adapt<IList<ComboGenerico>>().OrderBy(x => x.Descripcion).ToList());
            return actividades;
        }

        public IList<ComboGenerico> GetComboMenues(string codigoEstructura, bool conOpcionTodos = false)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Descripcion"
            };
            Search searchMenues = new Search(typeof(MenuOpcion));
            searchMenues.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchMenues.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
            List<ComboGenerico> menues = new List<ComboGenerico>();

            if (conOpcionTodos)
            {
                menues.Add(new ComboGenerico()
                {
                    Id = 0,
                    Codigo = "0",
                    Descripcion = "-- TODAS --"
                });
            }

            menues.AddRange(GetByCriteria<MenuOpcion>(searchMenues, propiedades).Adapt<IList<ComboGenerico>>().OrderBy(x => x.Descripcion).ToList());
            return menues;
        }

        public IList<ComboGenerico> GetComboNodos(string codigoEstructura, string codRol = "",bool conOpcionTodos = false)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "Descripcion"
            };
            Search searchNodos = new Search(typeof(NodoFuncional));
            searchNodos.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchNodos.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));

            if (codRol != "")
            {
                Search searchRol = new Search(typeof(Rol));
                searchRol.AddExpression(Restrictions.Eq("Codigo", codRol));
                Rol rol = GetByCriteria<Rol>(searchRol).FirstOrDefault();

                if (rol.TipoNodoFuncional != null) //si es null es multiple tipo nodo.
                {   
                    //sino se busca por tipo de nodo
                    searchNodos.AddAlias(new KeyValuePair<string, string>("TipoNodoFuncional", "TipoNodoFuncional"));
                    searchNodos.AddExpression(Restrictions.Eq("TipoNodoFuncional.Id", rol.TipoNodoFuncional.Id));
                }
            }

            List<ComboGenerico> nodos = new List<ComboGenerico>();

            if (conOpcionTodos)
            {
                nodos.Add(new ComboGenerico()
                {
                    Id = 0,
                    Codigo = "0",
                    Descripcion = "-- TODAS --"
                });
            }

            nodos.AddRange(GetByCriteria<NodoFuncional>(searchNodos, propiedades).Adapt<IList<ComboGenerico>>().OrderBy(x => x.Descripcion).ToList());
            return nodos;
        }

        /// <summary>
        /// Metodo que trae activiades segun un array de codigos de actividad
        /// </summary>
        /// <param name="codigos"></param>
        /// <returns></returns>
        protected IList<Actividad> GetActividadesPorCodigo(string[] codigos, string estructura)
        {
            Search searchActividades = new Search(typeof(Actividad));
            searchActividades.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchActividades.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            searchActividades.AddExpression(Restrictions.In("Codigo", codigos));
            return GetByCriteria<Actividad>(searchActividades);
        }

        /// <summary>
        /// Metodo que trae una actividad segun el codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        protected Actividad GetActividadPorCodigo(string codigo, string estructura, ISession session = null)
        {
            Search searchActividades = new Search(typeof(Actividad));
            searchActividades.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchActividades.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            searchActividades.AddExpression(Restrictions.Eq("Codigo", codigo));
            if (session != null)
                return GetByCriteria<Actividad>(searchActividades, session).SingleOrDefault();
            else
                return GetByCriteria<Actividad>(searchActividades).SingleOrDefault();
        }

        /// <summary>
        /// Metodo que trae una estructura segun el codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        protected EstructuraFuncional GetEstructuraPorCodigo(string codigo, ISession session = null)
        {
            Search searchEstructura = new Search(typeof(EstructuraFuncional));
            searchEstructura.AddExpression(Restrictions.Eq("Codigo", codigo));
            if (session != null)
                return GetByCriteria<EstructuraFuncional>(searchEstructura, session).SingleOrDefault();
            else
                return GetByCriteria<EstructuraFuncional>(searchEstructura).SingleOrDefault();
        }

        protected TipoNodoFuncional GetTipoNodoFuncionalPorCodigo(string codigo, ISession session = null)
        {
            Search search = new Search(typeof(TipoNodoFuncional));
            search.AddExpression(Restrictions.Eq("Codigo", codigo));
            if (session != null)
                return GetByCriteria<TipoNodoFuncional>(search, session).SingleOrDefault();
            else
                return GetByCriteria<TipoNodoFuncional>(search).SingleOrDefault();
        }

        /// <summary>
        /// Metodo que trae una estructura segun el codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public string GetDescripcionEstructura(string codigo)
        {
            return this.GetEstructuraPorCodigo(codigo).DescripcionEstructura;
        }

        /// <summary>
        /// Metodo que trae una estructura segun el codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        protected Usuario GetUsuarioPorNombreUsuario(string nombreUsuario, ISession session = null)
        {
            Search searchUsuario = new Search(typeof(Usuario));
            searchUsuario.AddExpression(Restrictions.Eq("NombreUsuario", nombreUsuario));
            if (session != null)
                return GetByCriteria<Usuario>(searchUsuario, session).SingleOrDefault();
            else
                return GetByCriteria<Usuario>(searchUsuario).SingleOrDefault();
        }

        /// <summary>
        /// Metodo que trae una estructura segun el codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        protected Usuario GetUsuarioPorNombreUsuario(string nombreUsuario, string tipoUsuario, ISession session = null)
        {
            if(tipoUsuario == UsuarioDTO.USUARIONOMINADO)
            {
                Search searchUsuarioNominado = new Search(typeof(Nominado));
                searchUsuarioNominado.AddExpression(Restrictions.Eq("NombreUsuario", nombreUsuario));
                if (session != null)
                    return GetByCriteria<Nominado>(searchUsuarioNominado, session).SingleOrDefault();
                else
                    return GetByCriteria<Nominado>(searchUsuarioNominado).SingleOrDefault();
            }
            else
            {
                Search searchUsuarioAcreedor = new Search(typeof(Acreedor));
                searchUsuarioAcreedor.AddExpression(Restrictions.Eq("NombreUsuario", nombreUsuario));
                if (session != null)
                    return GetByCriteria<Acreedor>(searchUsuarioAcreedor, session).SingleOrDefault();
                else
                    return GetByCriteria<Acreedor>(searchUsuarioAcreedor).SingleOrDefault();
            }

                

            
           
        }

        /// <summary>
        /// Metodo que trae un rol segun el codigo y la estructura
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        protected Rol GetRolPorCodigo(string codigo, string estructura, ISession session = null)
        {
            Search searchRol = new Search(typeof(Rol));
            searchRol.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchRol.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            searchRol.AddExpression(Restrictions.Eq("Codigo", codigo));
            if (session != null)
                return GetByCriteria<Rol>(searchRol, session).SingleOrDefault();
            else
                return GetByCriteria<Rol>(searchRol).SingleOrDefault();
        }

        /// <summary>
        /// Metodo que trae un menu segun el codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        protected MenuOpcion GetMenuPorCodigo(string codigo, string estructura, ISession session = null)
        {
            Search searchMenu = new Search(typeof(MenuOpcion));
            searchMenu.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchMenu.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            searchMenu.AddExpression(Restrictions.Eq("Codigo", codigo));
            if (session != null)
                return GetByCriteria<MenuOpcion>(searchMenu, session).SingleOrDefault();
            else
                return GetByCriteria<MenuOpcion>(searchMenu).SingleOrDefault();
        }

        /// <summary>
        /// Metodo que trae un nodo segun el codigo
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        protected NodoFuncional GetNodoPorCodigo(string codigo, string estructura, ISession session = null)
        {
            Search searchNodo = new Search(typeof(NodoFuncional));
            searchNodo.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchNodo.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            searchNodo.AddExpression(Restrictions.Eq("Codigo", codigo));
            if (session != null)
                return GetByCriteria<NodoFuncional>(searchNodo, session).SingleOrDefault();
            else
                return GetByCriteria<NodoFuncional>(searchNodo).SingleOrDefault();
        }
        
        #endregion


        protected long GetIdSecuencia(Type t)
        {
            ISession session = Session();
            string query = "";
            if(t == typeof(Actividad))
                query = "SELECT pk_tgp.sp_obtener_secuencia('SEG_ACTIVIDAD') FROM dual";
            else if(t == typeof(EstructuraFuncional))
                query = "SELECT pk_tgp.sp_obtener_secuencia('SEG_ESTRUCTURA_FUNCIONAL') FROM dual";
            else if (t == typeof(NodoFuncional))
                query = "SELECT pk_tgp.sp_obtener_secuencia('SEG_NODO_FUNCIONAL') FROM dual";
            else if (t == typeof(Rol))
                query = "SELECT pk_tgp.sp_obtener_secuencia('SEG_ROL') FROM dual";
            else if (t == typeof(MenuOpcion))
                query = "SELECT pk_tgp.sp_obtener_secuencia('SEG_MENU_OPCION') FROM dual";
            else if(t == typeof(AdministradorLocal))
                query = "SELECT pk_tgp.sp_obtener_secuencia('SEG_ADMINISTRADOR_LOCAL') FROM dual";
            else if(t == typeof(RolDelegadoAdmin))
                query = "SELECT pk_tgp.sp_obtener_secuencia('SEG_ROL_DELEGADO_ADMIN') FROM dual";


            var nextId = session.CreateSQLQuery(query).UniqueResult();
            return Convert.ToInt64(nextId);
        }

        /// <summary>
        /// Metodo que valida si la estructura puede borrarse
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected bool ValidaEliminacionEstructura(long id)
        {
            //Un search por cada entidad que pueda ser relacionada
            Search searchRoles = new Search(typeof(Rol));
            Search searchActividades = new Search(typeof(Actividad));
            Search searchNodos = new Search(typeof(NodoFuncional));
            Search searchMenues = new Search(typeof(MenuOpcion));
            Search searchNovedades = new Search(typeof(Novedad));

            List<Search> buscadorDeElementos = new List<Search>()
            {
                searchActividades, searchMenues, searchNodos, searchNovedades, searchRoles
            };

            //A cada search le agregamos el alias y la expression correspondiente
            foreach (Search search in buscadorDeElementos)
            {
                search.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
                search.AddExpression(Restrictions.Eq("EstructuraFuncional.Id", id));
            }

            //Lanzamos una excepcion si tiene algun elemento relacionado
            if (GetCountByCriteria<Rol>(searchRoles) > 0)
                throw new Exception("Debe eliminar los roles asociados a la estructura.");
            if (GetCountByCriteria<Actividad>(searchActividades) > 0)
                throw new Exception("Debe eliminar las actividades asociadas a la estructura.");
            if (GetCountByCriteria<Novedad>(searchNovedades) > 0)
                throw new Exception("Debe eliminar las novedades asociadas a la estructura.");
            if (GetCountByCriteria<NodoFuncional>(searchNodos) > 0)
                throw new Exception("Debe eliminar los nodos asociados a la estructura.");
            if (GetCountByCriteria<MenuOpcion>(searchMenues) > 0)
                throw new Exception("Debe eliminar los menues asociados a la estructura.");

            //Sino devolvemos true
            return true;
        }

        public string SolicitarToken()
        {
            return repository.Session.QueryOver<SolicitudToken>().Where(z => z.Nombre == ConfigurationManager.AppSettings["NOMBRE_TOKEN"]).OrderBy(x => x.CId).Desc.List<SolicitudToken>().FirstOrDefault().Valor;
        }

        // se agrega metodo para recuperar apps mas usadas...

        public List<ResponseAppsMasUsadas> GetAppsMasUsadas(string usuario, string appActual)
        {
            try
            {
                string metodo = "api/seguridad/aplicacionesMasUsadas";
                using (HttpClient client = new HttpClient())
                {
                    // Call asynchronous network methods in a try/catch block to handle exceptions
                    //Request para el comparar
                    RequestAPIAppsMasUsadas request = new RequestAPIAppsMasUsadas()
                    {
                        NombreUsuario = usuario,
                        CodigoEstructura = appActual
                    };
                    //Serializamos el request, con esta opcion evitamos las referencias circulares
                    string requestJson = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    StringContent content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(ConfigurationManager.AppSettings["UrlApiSegNuevaNovedad"].ToString() + metodo, content).Result;
                    response.EnsureSuccessStatusCode();
                    ResponseAPIAppsMasUsadas responseApi = JsonConvert.DeserializeObject<ResponseAPIAppsMasUsadas>(response.Content.ReadAsStringAsync().Result);
                    return responseApi.Apps;
                };
            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }




    }
}
