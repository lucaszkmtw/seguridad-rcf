
using Mapster;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using TGP.Seguridad.BussinessLogic.APIRequests;
using TGP.Seguridad.BussinessLogic.APIResponses;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Mapping;
using HelperService = TGP.Seguridad.DataAccess.Helpers.HelperService;

namespace TGP.Seguridad.BussinessLogic
{
    public class ComparadorApplicationService : SeguridadApplicationService
    {
        private static string MetodoComparar = "getelementosdestino";
        private static string MetodoCopiarRoles = "copiarroldestino";
        private static string MetodoCopiarActividades = "copiaractividaddestino";
        private static string MetodoEliminar = "eliminarelementodestino";
        private static string MetodoCopiarMenues = "copiarmenuesdestino";
        private static string MetodoCopiarNodos = "copiarnodosdestino";
        private static string MetodoCopiarEstructuras = "copiarestructurasdestino";


        private static ComparadorApplicationService instance;
        private static ISession sessionDestino;

        //Singleton
        public static new ComparadorApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ComparadorApplicationService();
                    //sessionDestino = instance.GetSessionPorConfig(ConfigurationManager.AppSettings["SeguridadDestino"]);
                }
                return instance;
            }
        }

        #region Generico

        public IList<ComboGenerico> GetComboEstructurasComparar(bool siConOpcionTodos = false)
        {
            string[] propiedades = new string[]
            {
                "Id",
                "Codigo",
                "DescripcionEstructura"
            };
            List<ComboGenerico> estructuras = new List<ComboGenerico>();
            if (siConOpcionTodos)
            {
                estructuras.Add(new ComboGenerico()
                {
                    Id = 0,
                    Codigo = "0",
                    Descripcion = "-- TODAS --"
                });
            }

            estructuras.AddRange(GetAll<EstructuraFuncional>(sessionDestino).Adapt<IList<ComboGenerico>>().OrderBy(x => x.Descripcion).ToList());
            return estructuras;
        }

        public IList<ElementoComparar> GetElementosDestino(string tipoComparador, string codigoEstructura)
        {
            try
            {
                switch (tipoComparador)
                {
                    case ComparadorGenerico.ROL:
                        return GetRolesComparar(codigoEstructura, true);
                    case ComparadorGenerico.MENUOPCION:
                        return GetMenuOpcionComparar(codigoEstructura, true);
                    case ComparadorGenerico.ACTIVIDAD:
                        return GetActividadesComparar(codigoEstructura, true);
                    case ComparadorGenerico.NODO:
                        return GetNodosComparar(codigoEstructura, true);
                    case ComparadorGenerico.ESTRUCTURA:
                        return GetEstructurasComparar(true);
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public IList<ElementoComparar> GetElementosOrigen(string tipoComparador, string codigoEstructura)
        {
            try
            {
                switch (tipoComparador)
                {
                    case ComparadorGenerico.ROL:
                        return GetRolesComparar(codigoEstructura);
                    case ComparadorGenerico.MENUOPCION:
                        return GetMenuOpcionComparar(codigoEstructura);
                    case ComparadorGenerico.ACTIVIDAD:
                        return GetActividadesComparar(codigoEstructura);
                    case ComparadorGenerico.NODO:
                        return GetNodosComparar(codigoEstructura);
                    case ComparadorGenerico.ESTRUCTURA:
                        return GetEstructurasComparar();
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public string CopiarDestino(string[] cod, string tipoComparador, string estructura)
        {
            try
            {
                switch (tipoComparador)
                {
                    case ComparadorGenerico.ROL:
                        CopiarRolDestino(cod, estructura);
                        return "El Rol se pasó con exito";
                    case ComparadorGenerico.MENUOPCION:
                        CopiarMenuDestino(cod, estructura);
                        return "El Menú se pasó con exito";
                    case ComparadorGenerico.ACTIVIDAD:
                        CopiarActividadDestino(cod, estructura);
                        return "La Actividad se pasó con exito";
                    case ComparadorGenerico.NODO:
                        CopiarNodosDestino(cod, estructura);
                        return "El Nodo se pasó con exito";
                    case ComparadorGenerico.ESTRUCTURA:
                        CopiarEstructurasDestino(cod);
                        return "La Estructura se pasó con exito";
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public string CopiarDestinoMasivo(string[] cod, string tipoComparador, string estructura)
        {

            try
            {
                switch (tipoComparador)
                {
                    case ComparadorGenerico.ROL:
                        CopiarRolDestino(cod, estructura);
                        return "Los Roles se pasaron con exito.";
                    case ComparadorGenerico.MENUOPCION:
                        CopiarMenuDestino(cod, estructura);
                        return "Los Menues se pasaron con exito.";
                    case ComparadorGenerico.ACTIVIDAD:
                        CopiarActividadDestino(cod, estructura);
                        return "Las Activdiades se pasaron con exito.";
                    case ComparadorGenerico.NODO:
                        CopiarNodosDestino(cod, estructura);
                        return "Los Nodos se pasaron con exito.";
                    case ComparadorGenerico.ESTRUCTURA:
                        CopiarEstructurasDestino(cod);
                        return "Las Estructuras se pasaron con exito.";
                    default:
                        return null;
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        /// <summary>
        /// Metodo que llama a la api para eliminar elementos en el destino
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="tipoComparador"></param>
        /// <returns></returns>
        public string EliminarDestino(long id, int version, string tipoComparador)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    // Call asynchronous network methods in a try/catch block to handle exceptions

                    //Request para el comparar
                    RequestEliminar request = new RequestEliminar()
                    {
                        NombreUsuario = HttpContext.Current.Session["usuario"].ToString(),
                        KeyEncrypt = Utils.Encriptador.Encriptar(ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString()),
                        Id = id,
                        TipoElemento = tipoComparador,
                        Version = version
                    };
                    //Serializamos el request, con esta opcion evitamos las referencias circulares
                    string requestJson = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    StringContent content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(ConfigurationManager.AppSettings["UrlApiDestino"].ToString() + MetodoEliminar, content).Result;
                    response.EnsureSuccessStatusCode();
                    ResponseAPI responseApi = JsonConvert.DeserializeObject<ResponseAPI>(response.Content.ReadAsStringAsync().Result);
                    return responseApi.Mensaje;
                };

            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }
        #endregion

        /// <summary>
        /// Obtiene los roles a comparar entre origen y destino
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <param name="esDestino">si es destino va sobre el proximo ambiente</param>
        /// <returns></returns>
        private IList<ElementoComparar> GetRolesComparar(string codigoEstructura, bool esDestino = false)
        {
            IList<Rol> roles;
            if (esDestino)
            {
                return GetElementosApiDestino(codigoEstructura, ComparadorGenerico.ROL);
            }
            else
            {
                roles = new List<Rol>();
                //ISession sessionDestino = GetSessionPorConfig(ConfigurationManager.AppSettings["SeguridadDestino"]);
                Search searchRoles = new Search(typeof(Rol));
                searchRoles.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
                string[] propiedades = new string[]
                {
                "Id",
                "Codigo",
                "Version",
                "Descripcion",
                "EstructuraFuncional.Codigo",
                "EstructuraFuncional.DescripcionEstructura",
                };


                roles = GetByCriteria<Rol>(searchRoles, propiedades);
                //sessionDestino.Close();
                return roles.Adapt<IList<ElementoComparar>>();
            }


        }



        /// <summary>
        /// Metodo que copia un rol al destino
        /// </summary>
        /// <param name="cod"></param>
        /// <param name="estructura"></param>
        /// <param name="esMasivo"></param>
        private void CopiarRolDestino(string[] cod, string estructura, bool esMasivo = false)
        {
            //Buscamos todos los roles que vamos a agregar/sobreescribir y los pasamos a RolDestino
            Search searchRoles = new Search(typeof(Rol));
            searchRoles.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchRoles.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            searchRoles.AddExpression(Restrictions.In("Codigo", cod));
            IList<RolDestino> rolesOrigen = GetByCriteria<Rol>(searchRoles).Adapt<IList<RolDestino>>();

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    // Call asynchronous network methods in a try/catch block to handle exceptions

                    //Request para el comparar
                    RequestCopiarRolDestino request = new RequestCopiarRolDestino()
                    {
                        NombreUsuario = HttpContext.Current.Session["usuario"].ToString(),
                        CodigoEstructuraFuncional = estructura,
                        KeyEncrypt = Utils.Encriptador.Encriptar(ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString()),
                        Roles = rolesOrigen
                    };
                    //Serializamos el request, con esta opcion evitamos las referencias circulares
                    string requestJson = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    ValidarResponse(client, request, MetodoCopiarRoles);

                };

            }
            catch (HttpRequestException e)
            {
                throw e;
            }

        }



        /// <summary>
        /// Metodo que obtiene los menues a comaprar entre un ambiente y otro
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <param name="esDestino">si es destino busca en el proximo ambiente</param>
        /// <returns></returns>
        private IList<ElementoComparar> GetMenuOpcionComparar(string codigoEstructura, bool esDestino = false)
        {
            if (esDestino)
            {
                return GetElementosApiDestino(codigoEstructura, ComparadorGenerico.MENUOPCION);
            }
            else
            {
                Search searchMenuOpcion = new Search(typeof(MenuOpcion));
                searchMenuOpcion.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
                string[] propiedades = new string[]
                {
                    "Id",
                    "Codigo",
                    "Version",
                    "Descripcion",
                    "MenuOpcionPadre.Descripcion",
                    "EstructuraFuncional.Codigo",
                    "EstructuraFuncional.DescripcionEstructura",
                };

                IList<MenuOpcion> menues;
                //sessionDestino = GetSessionPorConfig(ConfigurationManager.AppSettings["SeguridadDestino"]);
                menues = GetByCriteria<MenuOpcion>(searchMenuOpcion, propiedades);
                return menues.Adapt<IList<ElementoComparar>>();
            }

        }


        /// <summary>
        /// Copia de un menu al ambiente de destino
        /// </summary>
        /// <param name="cod"></param>
        /// <param name="estructura"></param>
        private void CopiarMenuDestino(string[] cod, string estructura)
        {
            //Buscamos todos los roles que vamos a agregar/sobreescribir y los pasamos a RolDestino
            Search searchMenues = new Search(typeof(MenuOpcion));
            searchMenues.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchMenues.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            searchMenues.AddExpression(Restrictions.In("Codigo", cod));
            IList<MenuDestino> menuesOrigen = GetByCriteria<MenuOpcion>(searchMenues).Adapt<IList<MenuDestino>>();

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    // Call asynchronous network methods in a try/catch block to handle exceptions

                    //Request para el comparar
                    RequestCopiarMenuDestino request = new RequestCopiarMenuDestino()
                    {
                        NombreUsuario = HttpContext.Current.Session["usuario"].ToString(),
                        CodigoEstructuraFuncional = estructura,
                        KeyEncrypt = Utils.Encriptador.Encriptar(ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString()),
                        Menues = menuesOrigen
                    };
                    //Serializamos el request, con esta opcion evitamos las referencias circulares
                    string requestJson = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    ValidarResponse(client, request, MetodoCopiarMenues);

                };

            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }



        /// <summary>
        /// Obtiene las actividades a comparar entre origen y destino
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <param name="esDestino">si es destino va sobre el proximo ambiente</param>
        /// <returns></returns>
        private IList<ElementoComparar> GetActividadesComparar(string codigoEstructura, bool esDestino = false)
        {
            if (esDestino)
            {
                return GetElementosApiDestino(codigoEstructura, ComparadorGenerico.ACTIVIDAD);
            }
            else
            {
                Search searchActividades = new Search(typeof(Actividad));
                searchActividades.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
                string[] propiedades = new string[]
                {
                "Id",
                "Codigo",
                "Version",
                "Descripcion",
                "EstructuraFuncional.Codigo",
                "EstructuraFuncional.DescripcionEstructura",
                };

                IList<Actividad> actividades;
                actividades = GetByCriteria<Actividad>(searchActividades, propiedades);
                return actividades.Adapt<IList<ElementoComparar>>();
            }

        }

        /// <summary>
        /// Metodo que copia un rol al destino
        /// </summary>
        /// <param name="cod"></param>
        /// <param name="estructura"></param>
        /// <param name="esMasivo"></param>
        private void CopiarActividadDestino(string[] cod, string estructura)
        {
            //if (sessionDestino == null)
            //    sessionDestino = GetSessionPorConfig(ConfigurationManager.AppSettings["SeguridadDestino"]);

            //Buscamos todos los roles que vamos a agregar/sobreescribir
            Search searchActividad = new Search(typeof(Actividad));
            searchActividad.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchActividad.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            searchActividad.AddExpression(Restrictions.In("Codigo", cod));
            IList<ActividadDestino> actividadesOrigen = GetByCriteria<Actividad>(searchActividad).Adapt<IList<ActividadDestino>>();

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    // Call asynchronous network methods in a try/catch block to handle exceptions

                    //Request para el comparar
                    RequestCopiarActividadDestino request = new RequestCopiarActividadDestino()
                    {
                        NombreUsuario = HttpContext.Current.Session["usuario"].ToString(),
                        CodigoEstructuraFuncional = estructura,
                        KeyEncrypt = Utils.Encriptador.Encriptar(ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString()),
                        Actividades = actividadesOrigen
                    };
                    //Serializamos el request, con esta opcion evitamos las referencias circulares
                    string requestJson = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    ValidarResponse(client, request, MetodoCopiarActividades);

                };

            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Obtiene los nodos a comparar entre origen y destino
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <param name="esDestino">si es destino va sobre el proximo ambiente</param>
        /// <returns></returns>
        private IList<ElementoComparar> GetNodosComparar(string codigoEstructura, bool esDestino = false)
        {
            if (esDestino)
            {
                return GetElementosApiDestino(codigoEstructura, ComparadorGenerico.NODO);
            }
            else
            {
                Search searchNodos = new Search(typeof(NodoFuncional));
                searchNodos.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", codigoEstructura));
                string[] propiedades = new string[]
                {
                "Id",
                "Codigo",
                "Version",
                "Descripcion",
                "EstructuraFuncional.Codigo",
                "EstructuraFuncional.DescripcionEstructura",
                };

                IList<NodoFuncional> nodos;
                nodos = GetByCriteria<NodoFuncional>(searchNodos, propiedades);
                return nodos.Adapt<IList<ElementoComparar>>();
            }

        }


        /// <summary>
        /// Metodo que copia un nodo al destino
        /// </summary>
        /// <param name="cod"></param>
        /// <param name="estructura"></param>
        /// <param name="esMasivo"></param>
        private void CopiarNodosDestino(string[] cod, string estructura)
        {
            //Buscamos todos los nodos que vamos a agregar/sobreescribir
            Search searchNodos = new Search(typeof(NodoFuncional));
            searchNodos.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
            searchNodos.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            searchNodos.AddExpression(Restrictions.In("Codigo", cod));
            IList<NodoDestino> nodosOrigen = GetByCriteria<NodoFuncional>(searchNodos).Adapt<IList<NodoDestino>>();

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    // Call asynchronous network methods in a try/catch block to handle exceptions

                    //Request para el comparar
                    RequestCopiarNodoDestino request = new RequestCopiarNodoDestino()
                    {
                        NombreUsuario = HttpContext.Current.Session["usuario"].ToString(),
                        CodigoEstructuraFuncional = estructura,
                        KeyEncrypt = Utils.Encriptador.Encriptar(ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString()),
                        Nodos = nodosOrigen
                    };
                    //Serializamos el request, con esta opcion evitamos las referencias circulares
                    string requestJson = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    ValidarResponse(client, request, MetodoCopiarNodos);


                };

            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Obtiene las estructuras a comparar entre origen y destino
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <param name="esDestino">si es destino va sobre el proximo ambiente</param>
        /// <returns></returns>
        private IList<ElementoComparar> GetEstructurasComparar(bool esDestino = false)
        {
            if (esDestino)
            {
                return GetElementosApiDestino(string.Empty, ComparadorGenerico.ESTRUCTURA);
            }
            else
            {
                string[] propiedades = new string[]
                {
                "Id",
                "Codigo",
                "Version",
                "DescripcionEstructura"
                };

                IList<EstructuraFuncional> estructuras;
                estructuras = GetAll<EstructuraFuncional>(propiedades);
                return estructuras.Adapt<IList<ElementoComparar>>();
            }

        }

        /// <summary>
        /// Metodo que copia un nodo al destino
        /// </summary>
        /// <param name="cod">CodigoEstructura</param>
        /// <param name="esMasivo"></param>
        private void CopiarEstructurasDestino(string[] cod)
        {
            //Buscamos todos los nodos que vamos a agregar/sobreescribir
            Search searchEstructuras = new Search(typeof(EstructuraFuncional));
            searchEstructuras.AddExpression(Restrictions.In("Codigo", cod));
            IList<EstructuraDestino> estructurasOrigen = GetByCriteria<EstructuraFuncional>(searchEstructuras).Adapt<IList<EstructuraDestino>>();

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    // Call asynchronous network methods in a try/catch block to handle exceptions

                    //Request para el comparar
                    RequestCopiarEstructuraDestino request = new RequestCopiarEstructuraDestino()
                    {
                        NombreUsuario = HttpContext.Current.Session["usuario"].ToString(),
                        KeyEncrypt = Utils.Encriptador.Encriptar(ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString()),
                        Estructuras = estructurasOrigen
                    };
                    ValidarResponse(client, request, MetodoCopiarEstructuras);

                };

            }
            catch (HttpRequestException e)
            {
                throw e;
            }

        }




        /// <summary>
        /// Metodo para obtener elementos desde la api de destino
        /// </summary>
        /// <param name="codigoEstructura"></param>
        /// <param name="tipoElemento"></param>
        /// <returns></returns>
        private static IList<ElementoComparar> GetElementosApiDestino(string codigoEstructura, string tipoElemento)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    // Call asynchronous network methods in a try/catch block to handle exceptions

                    //Request para el comparar
                    RequestComparar request = new RequestComparar()
                    {
                        NombreUsuario = HttpContext.Current.Session["usuario"].ToString(),
                        CodigoEstructuraFuncional = codigoEstructura,
                        KeyEncrypt = Utils.Encriptador.Encriptar(ConfigurationManager.AppSettings["CLAVE_SERVICIO"].ToString()),
                        TipoElemento = tipoElemento

                    };
                    //Serializamos el request
                    string requestJson = JsonConvert.SerializeObject(request);
                    StringContent content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(ConfigurationManager.AppSettings["UrlApiDestino"].ToString() + MetodoComparar, content).Result;
                    response.EnsureSuccessStatusCode();
                    //Leemos la respuesta y la transformamos en un response
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    ResponseGetEntidades r = (ResponseGetEntidades)JsonConvert.DeserializeObject(responseString, typeof(ResponseGetEntidades));
                    if (r.Codigo != ResponseGetEntidades.CodigoResponseSuccess)
                    {
                        throw new Exception("Codigo: " + r.Codigo + ". " + r.Mensaje);
                    }
                    return r.Elementos;
                }

            }
            catch (HttpRequestException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Metodo que valida al respuesta segun nuestros codigos de respuesta para la api seg
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        private static void ValidarResponse(HttpClient client, RequestAPI request, string metodo)
        {
            //Serializamos el request, con esta opcion evitamos las referencias circulares
            string requestJson = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            StringContent content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(ConfigurationManager.AppSettings["UrlApiDestino"].ToString() + metodo, content).Result;
            response.EnsureSuccessStatusCode();
            ResponseAPI responseApi = JsonConvert.DeserializeObject<ResponseAPI>(response.Content.ReadAsStringAsync().Result);
            if (responseApi.Codigo != "200")
                throw new Exception(responseApi.Mensaje);
        }


        #region Api


        /// <summary>
        /// Metodo que elimina una estructura del destino
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public void EliminarEstructuraDestino(long id, int version)
        {
            //NodoFuncional nodo = GetById<NodoFuncional>(id, new string[] { "Id"});
            int versionBase = GetVersion<EstructuraFuncional>(id, sessionDestino);

            //Si el version no coincide elevamos excepcion
            if (versionBase != version)
                throw new Exception("La estructura fue modificado por otro usuario.");

            using (var trx = BeginTransaction())
            {
                try
                {
                    if (ValidaEliminacionEstructura(id))
                    {
                        Delete<EstructuraFuncional>(id);
                        trx.Commit();
                    }


                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }
        }



        /// <summary>
        /// Metodo que copia un nodo al destino
        /// </summary>
        /// <param name="cod">CodigoEstructura</param>
        /// <param name="esMasivo"></param>
        public void CopiarEstructurasDestinoApi(IList<EstructuraDestino> estructurasOrigen)
        {

            using (var trx = BeginTransaction())
            {
                try
                {
                    //Nodo destino
                    EstructuraFuncional estructuraDestino;

                    foreach (EstructuraDestino estructuraOrigen in estructurasOrigen)
                    {
                        //Buscamos si exite el nodo en el destino
                        estructuraDestino = GetById<EstructuraFuncional>(estructuraOrigen.Id);//GetEstructuraPorCodigo(estructuraOrigen.Codigo, sessionDestino);

                        if (estructuraDestino is null)
                        {
                            if (ExisteCodigoElemento<EstructuraFuncional>(estructuraOrigen.Codigo))
                                throw new Exception("Ya existe en el destino una estructura con el codigo " + estructuraOrigen.Codigo + ".");

                            estructuraDestino = new EstructuraFuncional();
                            estructuraDestino.Id = estructuraOrigen.Id;
                            estructuraDestino.Usuario = GetUsuarioPorNombreUsuario(estructuraOrigen.NombreUsuario);

                        }
                        //Agregamos o modificamos los campos que si son editables

                        estructuraDestino.Codigo = estructuraOrigen.Codigo;
                        estructuraDestino.DescripcionEstructura = estructuraOrigen.DescripcionEstructura;
                        estructuraDestino.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
                        estructuraDestino.SiBloqueado = estructuraOrigen.SiBloqueado;

                        //Si el usuario no existe en destino por defecto ponemos usertgp
                        if (estructuraDestino.Usuario is null)
                            estructuraDestino.Usuario = GetUsuarioPorNombreUsuario("usertgp");

                        //Hacemos !esMasivo, porque si es masivo mandamos false al parametro de commit ya que se comittea en la transaccion
                        //del metodo que llama a este
                        SaveOrUpdate(estructuraDestino);
                    }
                    trx.Commit();
                }
                catch (Exception e)
                {
                    trx.Rollback();
                    throw e;
                }
            }
        }

        /// <summary>
        /// Metodo que elimina un nodo del destino
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public void EliminarNodoDestino(long id, int version)
        {
            int versionBase = GetVersion<NodoFuncional>(id, sessionDestino);

            //Si el version no coincide elevamos excepcion
            if (versionBase != version)
                throw new Exception("El Nodo fue modificado por otro usuario.");

            using (var trx = BeginTransaction())
            {
                try
                {
                    Search searchRolNodosUsuario = new Search(typeof(RolNodoUsuario));
                    searchRolNodosUsuario.AddAlias(new KeyValuePair<string, string>("NodoFuncional", "NodoFuncional"));
                    searchRolNodosUsuario.AddExpression(Restrictions.Eq("NodoFuncional.Id", id));
                    IList<RolNodoUsuario> rolesNodoUsuario = GetByCriteria<RolNodoUsuario>(searchRolNodosUsuario, new string[] { "Id" });
                    if (rolesNodoUsuario.Count > 0)
                        DeleteAll<RolNodoUsuario>(rolesNodoUsuario.Select(x => x.Id).ToList());

                    Delete<NodoFuncional>(id);
                    trx.Commit();
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }
        }


        /// <summary>
        /// Metodo que copia un nodo al destino
        /// </summary>
        /// <param name="cod"></param>
        /// <param name="estructura"></param>
        /// <param name="esMasivo"></param>
        public void CopiarNodosDestinoApi(IList<NodoDestino> nodosOrigen)
        {

            using (var trx = BeginTransaction(sessionDestino))
            {
                try
                {
                    //Nodo destino
                    NodoFuncional nodoDestino;

                    foreach (NodoDestino nodoOrigen in nodosOrigen)
                    {
                        //Buscamos si exite el nodo en el destino
                        nodoDestino = GetById<NodoFuncional>(nodoOrigen.Id);


                        //Si no existe creamos uno neuvo y seteamos estructura y codigo, sino estos se mantienen porque no podemos comparar cruzando estructuras ni editar el codigo
                        if (nodoDestino is null)
                        {


                            nodoDestino = new NodoFuncional();
                            nodoDestino.Id = nodoOrigen.Id;
                            nodoDestino.Usuario = GetUsuarioPorNombreUsuario(nodoOrigen.NombreUsuario);


                        }
                        //Agregamos o modificamos los campos que si son editables
                        nodoDestino.EstructuraFuncional = GetById<EstructuraFuncional>(nodoOrigen.IdEstructuraFuncional) ?? throw new Exception("Debe pasar la estructura primero");
                        nodoDestino.TipoNodoFuncional = GetById<TipoNodoFuncional>(nodoOrigen.IdTipoNodoFuncional) ?? throw new Exception("El tipo de nodo funcional no se encuentra en el destino.");

                        //if (ExisteCodigoNodo(nodoDestino.Codigo, nodoDestino.EstructuraFuncional.Codigo) && nodoDestino.Codigo != nodoOrigen.Codigo)
                        //    throw new Exception("Ya existe en el destino un nodo con el codigo " + nodoOrigen.Codigo + ".");

                        if (ExisteCodigoNodo(nodoOrigen.Codigo, nodoDestino.EstructuraFuncional.Codigo) && nodoDestino.Codigo != nodoOrigen.Codigo)
                            throw new Exception("Ya existe en el destino un nodo con el codigo " + nodoOrigen.Codigo + ".");


                        nodoDestino.Codigo = nodoOrigen.Codigo;
                        nodoDestino.Descripcion = nodoOrigen.Descripcion;
                        nodoDestino.FechaUltimaOperacion = HelperService.Instance.GetDateToday();

                        //Si el nodo origen no tiene padre
                        if (nodoOrigen.IdNodoPadre != null)
                        {
                            //Si el nodo destino es null o es diferente al nodo padre del origen lo agregamos
                            if (nodoDestino.NodoFuncionalPadre == null || nodoDestino.NodoFuncionalPadre.Id != nodoOrigen.IdNodoPadre)
                            {
                                //Si no existe en el destino, sino, tiramos una excepcion 
                                NodoFuncional nodoPadreDestino = GetById<NodoFuncional>((long)nodoOrigen.IdNodoPadre);
                                if (nodoPadreDestino != null)
                                    nodoDestino.NodoFuncionalPadre = nodoPadreDestino;
                                else
                                    throw new Exception("El nodo padre debe pasarse primero.");
                            }
                        }
                        //sino, simplemente ponemos null en nodo padre
                        else
                            nodoDestino.NodoFuncionalPadre = null;

                        //Si el usuario no existe en destino por defecto ponemos usertgp
                        if (nodoDestino.Usuario is null)
                            nodoDestino.Usuario = GetUsuarioPorNombreUsuario("usertgp", sessionDestino);

                        //Hacemos !esMasivo, porque si es masivo mandamos false al parametro de commit ya que se comittea en la transaccion
                        //del metodo que llama a este
                        SaveOrUpdate(nodoDestino);
                        Session().Flush();
                    }
                    trx.Commit();
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Metodo que elimina una actividad en el ambiente destino
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public void EliminarActividadDestino(long id, int version)
        {
            Actividad actividadDestino = GetById<Actividad>(id, sessionDestino);

            //Si el version no coincide elevamos excepcion
            if (actividadDestino.Version != version)
                throw new Exception("La actividad fue modificada por otro usuario.");


            //Search de roles actividad
            Search searchRolesActivdiad = new Search(typeof(RolActividad));
            searchRolesActivdiad.AddAlias(new KeyValuePair<string, string>("Actividad", "Actividad"));
            searchRolesActivdiad.AddExpression(Restrictions.Eq("Actividad.Id", id));

            //Search de menues actividad
            Search searchActividadMenu = new Search(typeof(ActividadMenuOpcion));
            searchActividadMenu.AddAlias(new KeyValuePair<string, string>("Actividad", "Actividad"));
            searchActividadMenu.AddExpression(Restrictions.Eq("Actividad.Id", id));

            //Si existen menues asociados a la actividad no permito borrar y elevo una excepcion
            IList<ActividadMenuOpcion> menuActividadDestino = GetByCriteria<ActividadMenuOpcion>(searchActividadMenu, new string[] { "IdActividad", "IdMenuOpcion" });
            if (menuActividadDestino.Count > 0)
                throw new Exception("La actividad se encuentra relacionada a un Menu, por favor verifique");

            IList<RolActividad> rolesActividadDestino = GetByCriteria<RolActividad>(searchRolesActivdiad, new string[] { "IdActividad", "IdRol" });
            using (var trx = BeginTransaction())
            {
                try
                {
                    //Si existe roles asociados a la actividada los borro
                    if (rolesActividadDestino.Count > 0)
                        DeleteAll<RolActividad>(rolesActividadDestino.Select(x => x.GetId()).ToList(), false);

                    Delete<Actividad>(id);
                    trx.Commit();
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Metodo que copia un rol al destino
        /// </summary>
        /// <param name="cod"></param>
        /// <param name="estructura"></param>
        /// <param name="esMasivo"></param>
        public void CopiarActividadDestinoApi(IList<ActividadDestino> actividadesOrigen)
        {

            using (var trx = BeginTransaction())
            {
                try
                {
                    //Actividad destino
                    Actividad actividadDestino;

                    foreach (ActividadDestino actividadOrigen in actividadesOrigen)
                    {
                        //Buscamos si exite la actividad en el destino
                        actividadDestino = GetById<Actividad>(actividadOrigen.Id);

                        //Si no existe creamos uno neuvo y seteamos estructura, sino estos se mantienen porque no podemos comparar cruzando estructuras ni editar el codigo
                        if (actividadDestino is null)
                        {
                            if (ExisteCodigoElemento<Actividad>(actividadOrigen.Codigo))
                                throw new Exception("Ya existe en el destino una actividad con el codigo " + actividadOrigen.Codigo + ".");

                            actividadDestino = new Actividad();
                            actividadDestino.Id = actividadOrigen.Id;
                            actividadDestino.EstructuraFuncional = GetById<EstructuraFuncional>(actividadOrigen.IdEstructuraFuncional) ?? throw new Exception("Debe pasar la estructura primero");

                        }

                        //Agregamos o modificamos los campos que si son editables
                        actividadDestino.Codigo = actividadOrigen.Codigo;
                        actividadDestino.Descripcion = actividadOrigen.Descripcion;
                        actividadDestino.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
                        actividadDestino.UsuarioResponsable = GetUsuarioPorNombreUsuario(actividadOrigen.NombreUsuario);

                        //Si el usuario no existe en destino por defecto ponemos usertgp
                        if (actividadDestino.UsuarioResponsable is null)
                            actividadDestino.UsuarioResponsable = GetUsuarioPorNombreUsuario("usertgp");

                        //Hacemos !esMasivo, porque si es masivo mandamos false al parametro de commit ya que se comittea en la transaccion
                        //del metodo que llama a este
                        SaveOrUpdate(actividadDestino);
                        Session().Flush();
                    }
                    trx.Commit();
                }
                catch (Exception e)
                {
                    trx.Rollback();
                    throw e;
                }
            }
        }



        /// <summary>
        /// Elimina un menu del ambiente destino
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public void EliminarMenuDestino(long id, int version)
        {
            MenuOpcion menu = GetById<MenuOpcion>(id);

            if (menu.Version != version)
                throw new Exception("El Menú fue modificado por otro usuario");

            using (var trx = BeginTransaction())
            {
                try
                {

                    if (menu.ActividadAsociada != null)
                    {
                        ActividadMenuOpcion act = menu.ActividadAsociada;
                        menu.ActividadAsociada = null;
                        Delete<ActividadMenuOpcion>(act.GetId());
                    }
                    Delete<MenuOpcion>(id);
                    trx.Commit();
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Copia de un menu al ambiente de destino
        /// </summary>
        /// <param name="cod"></param>
        /// <param name="estructura"></param>
        /// <param name="esMasivo"></param>
        public void CopiarMenuDestinoApi(IList<MenuDestino> menuesOrigen)
        {
            //Menu destino
            MenuOpcion menuDestino;

            using (var trx = BeginTransaction())
            {
                try
                {
                    foreach (MenuDestino menuOrigen in menuesOrigen)
                    {
                        //Buscamos si exite el menu en el destino
                        menuDestino = GetById<MenuOpcion>(menuOrigen.Id);

                        //Si no existe creamos uno nuevo y seteamos estructura y codigo, sino estos se mantienen porque no podemos comparar cruzando estructuras ni editar el codigo
                        if (menuDestino is null)
                        {
                            menuDestino = new MenuOpcion();
                            menuDestino.Id = menuOrigen.Id;
                            menuDestino.EstructuraFuncional = GetById<EstructuraFuncional>(menuOrigen.IdEstructuraFuncional) ??
                                throw new Exception("Debe pasar la estructura primero");
                            menuDestino.Codigo = menuOrigen.Codigo;
                        }

                        //Agregamos o modificamos los campos que si son editables
                        menuDestino.Descripcion = menuOrigen.Descripcion;
                        menuDestino.Icono = menuOrigen.Icono;
                        menuDestino.NumeroOrden = menuOrigen.NumeroOrden;
                        menuDestino.Url = menuOrigen.Url;
                        menuDestino.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
                        menuDestino.UsuarioResponable = GetUsuarioPorNombreUsuario(menuOrigen.NombreUsuario);

                        //Si el menu padre es nulo lo guardo nulo, sino voy a buscarlo
                        if (menuOrigen.IdMenuOpcionPadre != null)
                            menuDestino.MenuOpcionPadre = GetById<MenuOpcion>((long)menuOrigen.IdMenuOpcionPadre) ??
                                throw new Exception("Debe pasar el menu padre primero.");
                        else
                            menuDestino.MenuOpcionPadre = null;

                        //Si el usuario no existe en destino por defecto ponemos usertgp
                        if (menuDestino.UsuarioResponable is null)
                            menuDestino.UsuarioResponable = GetUsuarioPorNombreUsuario("usertgp", sessionDestino);

                        //Armamos una transaccion porque para poder quitar la asociasion con la actividad tenemos que borrar el registro y hacer update sobre
                        //el menu de opcion, luego hacer save or update



                        SaveOrUpdate(menuDestino);
                        //Session().Flush();
                        //Si tenia actividades las borramos
                        if (menuOrigen.IdActividad == null && menuDestino.ActividadAsociada != null || (menuOrigen.IdActividad != null && menuDestino.ActividadAsociada != null))
                        {
                            if (menuDestino.ActividadAsociada != null && menuDestino.ActividadAsociada.IdActividad != menuOrigen.IdActividad)
                            {
                                ActividadMenuOpcion act = menuDestino.ActividadAsociada;
                                menuDestino.ActividadAsociada = null;
                                Delete<ActividadMenuOpcion>(act.GetId());
                                //Session().Flush();
                                //SaveOrUpdate(menuDestino, false, sessionDestino);
                            }

                        }
                        //Sino simplemente instanciamos una nueva
                        if (menuOrigen.IdActividad != null)
                        {
                            if (menuDestino.ActividadAsociada == null || menuDestino.ActividadAsociada.IdActividad != menuOrigen.IdActividad)
                            {
                                Actividad a = GetById<Actividad>((long)menuOrigen.IdActividad) ??
                                        throw new Exception("Debe pasar la actividad asociada primero.");

                                //Vamos a agregar una nueva asociacion
                                //menuDestino.ActividadAsociada = new ActividadMenuOpcion();
                                ActividadMenuOpcion m = new ActividadMenuOpcion();
                                m.IdActividad = a.Id;
                                m.IdMenuOpcion = menuDestino.Id;
                                m.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
                                m.UsuarioAlta = menuDestino.UsuarioResponable;
                                //menuDestino.ActividadAsociada = m;
                                SaveOrUpdate(m);
                            }

                        }
                        Session().Flush();
                    }
                    trx.Commit();
                }
                catch (Exception e)
                {
                    trx.Rollback();
                    throw e;
                }
            }
        }




        /// <summary>
        /// Metodo que elimina un rol del destino
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public void EliminarRolDestino(long id, int version)
        {

            //Buscamos el version q esta en la base, si no coincide lanzamos excepcion
            //int versionBase = GetVersion<Rol>(id);
            Rol rol = GetById<Rol>(id);
            if (rol.Version != version)
                throw new Exception("El Rol fue modificado por otro usuario");

            string[] propiedades = new string[]
            {
                "Id"
            };

            Search searchRolesNodoUsuario = new Search(typeof(RolNodoUsuario));
            searchRolesNodoUsuario.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchRolesNodoUsuario.AddExpression(Restrictions.Eq("Rol.Id", id));
            IList<long> rolesNodoUsuario = GetByCriteria<RolNodoUsuario>(searchRolesNodoUsuario, propiedades, null, false).Select(x => x.Id).ToList();

            using (var trx = BeginTransaction())
            {
                try
                {
                    DeleteAll<RolNodoUsuario>(rolesNodoUsuario, false);
                    Delete<Rol>(id);
                    trx.Commit();
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Metodo que copia un rol al destino
        /// </summary>
        /// <param name="cod"></param>
        /// <param name="estructura"></param>
        /// <param name="esMasivo"></param>
        public void CopiarRolDestinoApi(IList<RolDestino> rolesOrigen, bool esMasivo = false)
        {

            using (var trx = BeginTransaction())
            {
                try
                {
                    //Rol destino
                    Rol rolDestino;
                    foreach (RolDestino rolOrigen in rolesOrigen)
                    {
                        //Buscamos si exite el rol en el destino
                        rolDestino = GetById<Rol>(rolOrigen.Id);//GetRolPorCodigo(rolOrigen.Codigo, rolOrigen.EstructuraFuncional.Codigo, sessionDestino);

                        //Si no existe creamos uno neuvo y seteamos estructura y codigo, sino estos se mantienen porque no podemos comparar cruzando estructuras ni editar el codigo
                        if (rolDestino is null)
                        {
                            rolDestino = new Rol();
                            rolDestino.Id = rolOrigen.Id;
                            rolDestino.Actividades = new List<RolActividad>();
                            rolDestino.EstructuraFuncional = GetById<EstructuraFuncional>(rolOrigen.IdEstructuraFuncional) ?? throw new Exception("Debe pasar la estructura primero");

                        }

                        //Agregamos o modificamos los campos que si son editables
                        rolDestino.Codigo = rolOrigen.Codigo;
                        rolDestino.SiDelegable = rolOrigen.SiDelegable;
                        rolDestino.InformacionRol = rolOrigen.InformacionRol;
                        rolDestino.Descripcion = rolOrigen.Descripcion;
                        rolDestino.FechaUltimaOperacion = HelperService.Instance.GetDateToday();
                        rolDestino.UsuarioAlta = GetUsuarioPorNombreUsuario(rolOrigen.NombreUsuario);
                        rolDestino.TipoNodoFuncional = GetById<TipoNodoFuncional>(rolOrigen.IdTipoNodoFuncional) ?? throw new Exception("Debe pasar el tipo de Nodo Funcional");
                        rolDestino.EsMultinodo = rolOrigen.EsMultinodo;

                        //Si el usuario no existe en destino por defecto ponemos usertgp
                        if (rolDestino.UsuarioAlta is null)
                            rolDestino.UsuarioAlta = GetUsuarioPorNombreUsuario("usertgp");


                        //Eliminamos todas las relaciones y volvemos a crearlas
                        rolDestino.Actividades.Clear();
                        //Hacemos un flush en el medio para que las relaciones estes eliminadas para la sesion (EL FLUSH NO HACE COMMIT)
                        Session().Flush();

                        foreach (long idActividad in rolOrigen.IdActividades)
                        {
                            //Si existe alguna actividad en el origen q no esta asociada en el destino agregamos la asociacion
                            if (!rolDestino.Actividades.Any(x => x.Rol.Id.Equals(rolOrigen.Id) && x.Actividad.Codigo.Equals(idActividad)))
                            {
                                Actividad a = GetById<Actividad>(idActividad) ?? throw new Exception("Debe pasar las actividades primero.");
                                Rol r = GetById<Rol>(rolOrigen.Id) ?? rolDestino;

                                //Creando un nuevo rolactividad
                                rolDestino.Actividades.Add(new RolActividad()
                                {
                                    IdActividad = a.Id,
                                    FechaUltimaOperacion = HelperService.Instance.GetDateToday(),
                                    UsuarioResponsable = rolDestino.UsuarioAlta,
                                    IdRol = r.Id,
                                    Rol = r,
                                    Actividad = a
                                });
                                SaveOrUpdate(rolDestino);
                            }
                        }
                        SaveOrUpdate(rolDestino);
                    }

                    trx.Commit();
                }
                catch (Exception e)
                {
                    trx.Rollback();
                    throw e;
                }
            }
        }
        #endregion

    }
}
