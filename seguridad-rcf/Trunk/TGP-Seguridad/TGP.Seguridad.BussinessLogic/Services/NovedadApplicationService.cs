
using Mapster;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using TGP.Seguridad.BussinessLogic.Generics;
using TGP.Seguridad.BussinessLogic.Helpers;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Mapping;
using TGP.Seguridad.Common.Dto;
using TGP.Seguridad.BussinessLogic.Dto.Usuario;
using TGP.Seguridad.BussinessLogic.Dto.Novedades;
using System.Globalization;

namespace TGP.Seguridad.BussinessLogic
{
    /// <summary>
    /// Clase de soporte para los tipos de novedades
    /// </summary>
    public class TipoNovedadConstants
    {
        public const string TIPO_NOVEDAD_NOTIFICACION = "NOTIFICACION";
        public const string TIPO_NOVEDAD_ADVERTENCIA = "ADVERTENCIA";
    }
    public class NovedadApplicationService : SeguridadApplicationService
    {
        private static NovedadApplicationService instance;

        //Singleton
        public new static NovedadApplicationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NovedadApplicationService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Metodo que obtiene las novedades
        /// </summary>
        /// <param name="estructura"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
        public IList<NovedadDTO> GetNovedades(string estructura, string rol, string fechaInicio, string fechaFin)
        {

            //Se busca por NovedadRolNodo porque no hay forma de armar un criteria que me permita saber
            //si una novedad tiene dentro de su lista de NovedadRolNodo un elemento que coincida con el rol seleccionado en el filtro
            string[] propiedades = new string[]
            {
                "Id",
                "Rol.Codigo",
                "Rol.Descripcion",
                "Novedad.Id",
                "Novedad.Descripcion",
                "Novedad.EstructuraFuncional.DescripcionEstructura",
                "Novedad.EstructuraFuncional.Codigo",
                "Novedad.FechaDesde",
                "Novedad.FechaHasta",
                "Novedad.NumeroVersion",
                "Novedad.TipoNovedad",
                "Novedad.SiPublica",
                "Novedad.Version",
                "NodoFuncional.Codigo",
                "NodoFuncional.Descripcion"

            };

            Search searchNovedades = new Search(typeof(NovedadRolNodo));

            searchNovedades.AddAlias(new KeyValuePair<string, string>("NodoFuncional", "NodoFuncional"));
            searchNovedades.AddAlias(new KeyValuePair<string, string>("NodoFuncional.EstructuraFuncional", "EstructuraFuncional"));
            searchNovedades.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            searchNovedades.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));

            //Si la estructura no es TODAS
            if (!string.IsNullOrEmpty(estructura) && !estructura.Equals(ComboGenerico.ComboVacio))
            {
                searchNovedades.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura));
            }
            //Si el rol no es TODOS
            if (!string.IsNullOrEmpty(rol) && !rol.Equals(ComboGenerico.ComboVacio))
            {
                searchNovedades.AddExpression(Restrictions.Eq("Rol.Codigo", rol));
            }

            //Implementancion de filtro por Fechas
            if (fechaInicio != "" && fechaFin != "")
            {
                Conjunction conjunction = Expression.Conjunction();
                conjunction.Add(Expression.Ge("Novedad.FechaDesde", DateTime.Parse(fechaInicio).Date))
                           .Add(Expression.Le("Novedad.FechaDesde", DateTime.Parse(fechaFin).Date));
                searchNovedades.AddExpression(conjunction);
            }
            //Traigo el listado de NovedadRolNodo y lo agrupo por ID de novedad. Luego cada grupo arma un viewmodel
            //para el listado
            IList<NovedadDTO> novedades = GetByCriteria<NovedadRolNodo>(searchNovedades, propiedades)
                .GroupBy(x => x.Novedad.Id)
                .Select(n => new NovedadDTO()
                {
                    Version = n.First().Novedad.Version,
                    Descripcion = n.First().Novedad.Descripcion,
                    EstructuraFuncionalCodigo = n.First().Novedad.EstructuraFuncional != null ? n.First().Novedad.EstructuraFuncional.Codigo : null,
                    EstructuraFuncionalDescripcion = n.First().Novedad.EstructuraFuncional != null ? n.First().Novedad.EstructuraFuncional.DescripcionEstructura : null,
                    FechaDesde = n.First().Novedad.FechaDesde,
                    FechaHasta = n.First().Novedad.FechaHasta,
                    Id = n.First().Novedad.Id,
                    NumeroVersion = n.First().Novedad.NumeroVersion ?? "",
                    SiPublica = n.First().Novedad.SiPublica,
                    TipoNovedad = n.First().Novedad.TipoNovedad ?? "",
                    NodosFuncionales = n.Select(d => d.NodoFuncional.Codigo).ToList(),
                }).ToList();

            //Si la estructura y el rol son TODOS debo traer ademas todas las novedades que no tienen
            //ninguna relacion con NovedadRolNodo, es decir las novedades publicas
            if (rol.Equals(ComboGenerico.ComboVacio) && estructura.Equals(ComboGenerico.ComboVacio))
            {
                string[] propiedadesNovedad = new string[]
                {
                    "Id",
                    "Version",
                    "SiPublica",
                    "Descripcion",
                    "TipoNovedad",
                    "FechaDesde",
                    "FechaHasta",
                    "NumeroVersion",
                    "EstructuraFuncional.Codigo"
                };
                searchNovedades = new Search(typeof(Novedad));
                searchNovedades.AddExpression(Restrictions.Eq("SiPublica", true));

                // filtro por fecha por Novedades Publicas 
                if (fechaInicio != "" && fechaFin != "")
                {
                    Conjunction conjunction = Expression.Conjunction();
                    conjunction.Add(Expression.Ge("FechaDesde", DateTime.Parse(fechaInicio).Date))
                        .Add(Expression.Le("FechaDesde", DateTime.Parse(fechaFin).Date));
                    searchNovedades.AddExpression(conjunction);
                }

                //Una vez q las traigo las adapto al viewmodel del listado y las sumo a las anteriores
                IList<Novedad> novedadesPublicas = GetByCriteria<Novedad>(searchNovedades, propiedadesNovedad);
                IList<NovedadDTO> novedadesPublicasViewModel = novedadesPublicas.Adapt<IList<NovedadDTO>>();
                if (novedadesPublicas != null && novedadesPublicas.Count > 0)
                    ((List<NovedadDTO>)novedades).AddRange(novedadesPublicasViewModel);
            }
            return novedades;
        }
        /// <summary>
        /// Metodo que inserta una nueva novedad
        /// </summary>
        /// <param name="novedad"></param>
        /// 

        private DateTime recuperarFechaInicio(string fechaInicio)
        {
            CultureInfo culture = new CultureInfo("es-AR");
            DateTime FechaInicioDate;
            if (fechaInicio != null)
                FechaInicioDate = DateTime.Parse(fechaInicio, culture);
            else
                FechaInicioDate = DateTime.Now;
            return FechaInicioDate;
        }

        private DateTime recuperarFechaFin(string fechaFin)
        {
            CultureInfo culture = new CultureInfo("es-AR");
            DateTime FechaFinDate;
            if (fechaFin != null)
                FechaFinDate = DateTime.Parse(fechaFin, culture);
            else
                FechaFinDate = DateTime.Now;

            FechaFinDate = FechaFinDate.AddDays(1).AddSeconds(-1);
            return FechaFinDate;
        }

        public void GuardarNovedad(NovedadDTO novedad)
        {

            //Creo la novedad sin la lista de rolnodonovedad
            Novedad nuevaNovedad = new Novedad()
            {
                Titulo = novedad.Titulo,
                Descripcion = novedad.Descripcion,
                FechaDesde = novedad.FechaDesde,
                FechaHasta = novedad.FechaHasta,
                NumeroVersion = novedad.NumeroVersion,
                SiPublica = novedad.SiPublica,
                TipoNovedad = novedad.TipoNovedad,
                ListaNovedadRolNodo = new List<NovedadRolNodo>()
            };

            //Si la novedad no es publica, debo ir a buscar la estructura y sus roles nodos
            if (!novedad.SiPublica)
            {
                //Get Estructura
                Search searchEstructura = new Search(typeof(EstructuraFuncional));
                searchEstructura.AddExpression(Restrictions.Eq("Codigo", novedad.EstructuraFuncionalCodigo));
                EstructuraFuncional estructura = GetByCriteria<EstructuraFuncional>(searchEstructura).SingleOrDefault();

                nuevaNovedad.EstructuraFuncional = estructura;

                //Get roles para novedad
                Search searchRoles = new Search(typeof(Rol));
                searchRoles.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
                searchRoles.AddExpression(Restrictions.In("Codigo", novedad.Roles.ToArray()));
                searchRoles.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura.Codigo));
                IList<Rol> roles = GetByCriteria<Rol>(searchRoles);

                //Get nodos para novedad
                Search searchNodos = new Search(typeof(NodoFuncional));
                searchNodos.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
                searchNodos.AddExpression(Restrictions.In("Codigo", novedad.NodosFuncionales.ToArray()));
                searchNodos.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", estructura.Codigo));
                IList<NodoFuncional> nodos = GetByCriteria<NodoFuncional>(searchNodos);

                //Por cada rol y luego por cada nodo seleccionado guardamos la relacion con la novedad
                NovedadRolNodo novedadRolNodo;
                foreach (Rol r in roles)
                {
                    foreach (NodoFuncional n in nodos)
                    {
                        novedadRolNodo = new NovedadRolNodo()
                        {
                            Novedad = nuevaNovedad,
                            Rol = r,
                            NodoFuncional = n
                        };
                        nuevaNovedad.ListaNovedadRolNodo.Add(novedadRolNodo);
                    }
                }
            }
            //Si la novedad es publica pongo null en estructura funcional porque no tiene relacion
            else
            {
                Search searchEstructura = new Search(typeof(EstructuraFuncional));
                searchEstructura.AddExpression(Restrictions.Eq("Codigo", NovedadDTO.NOVEDADTODOS));
                EstructuraFuncional estructura = GetByCriteria<EstructuraFuncional>(searchEstructura).SingleOrDefault();

                nuevaNovedad.EstructuraFuncional = estructura;
            }
            Insert(nuevaNovedad);
        }

        /// <summary>
        /// Metodo para guardar la edicion de una novedad
        /// </summary>
        /// <param name="novedad"></param>
        public void GuardarEdicionNovedad(NovedadDTO novedad)
        {
            //Obtenemos la noevdad de la base
            Novedad novedadBase = GetById<Novedad>(novedad.Id);
            //Si el version no coincide tiramos una esxcepcion
            if (novedad.Version != novedadBase.Version)
                throw new Exception("La novedad ya fue modificada por otro usuario");

            //Si la novedad no es publica vamos a tomar la estructura seleccionada
            if (!novedad.SiPublica)
            {
                //Get Estructura
                Search searchEstructura = new Search(typeof(EstructuraFuncional));
                searchEstructura.AddExpression(Restrictions.Eq("Codigo", novedad.EstructuraFuncionalCodigo));
                novedadBase.EstructuraFuncional = GetByCriteria<EstructuraFuncional>(searchEstructura).SingleOrDefault();
            }

            //Copiamos todas las propiedades del viewmodel de novedad en la entidad novedad
            novedadBase.Titulo = novedad.Titulo;
            novedadBase.Descripcion = novedad.Descripcion;
            novedadBase.FechaDesde = novedad.FechaDesde;
            novedadBase.FechaHasta = novedad.FechaHasta;
            novedadBase.NumeroVersion = novedad.NumeroVersion;
            novedadBase.SiPublica = novedad.SiPublica;
            novedadBase.TipoNovedad = novedad.TipoNovedad;

            //Si la novedad es publica ponemos null en su estructura y limpiamos cualquier relacion NovedadRolNodo que haya existido
            if (novedad.SiPublica)
            {
                novedadBase.EstructuraFuncional = null;
                novedadBase.ListaNovedadRolNodo.Clear();
                Update(novedadBase);
            }
            //Si no es publica vamos a buscar todos los roles y nodos seleccionados
            else
            {
                //Get roles para novedad
                Search searchRoles = new Search(typeof(Rol));
                searchRoles.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
                searchRoles.AddExpression(Restrictions.In("Codigo", novedad.Roles.ToArray()));
                searchRoles.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", novedadBase.EstructuraFuncional.Codigo));
                IList<Rol> roles = GetByCriteria<Rol>(searchRoles);

                //Get nodos para novedad
                Search searchNodos = new Search(typeof(NodoFuncional));
                searchNodos.AddAlias(new KeyValuePair<string, string>("EstructuraFuncional", "EstructuraFuncional"));
                searchNodos.AddExpression(Restrictions.In("Codigo", novedad.NodosFuncionales.ToArray()));
                searchNodos.AddExpression(Restrictions.Eq("EstructuraFuncional.Codigo", novedadBase.EstructuraFuncional.Codigo));
                IList<NodoFuncional> nodos = GetByCriteria<NodoFuncional>(searchNodos);

                //Abrimos una transaccion
                using (var trx = BeginTransaction())
                {
                    try
                    {
                        //Limpiamos todas las relaciones y hacemos un update sin commit
                        novedadBase.ListaNovedadRolNodo.Clear();
                        Update(novedadBase);

                        //Por cada rol y por cada nodod guardamos la relacion en la novedad
                        NovedadRolNodo novedadRolNodo;
                        foreach (Rol r in roles)
                        {
                            foreach (NodoFuncional n in nodos)
                            {
                                novedadRolNodo = new NovedadRolNodo()
                                {
                                    Novedad = novedadBase,
                                    Rol = r,
                                    NodoFuncional = n
                                };
                                novedadBase.ListaNovedadRolNodo.Add(novedadRolNodo);
                            }
                        }
                        //Hacemos update nuevamente sobre la novedad y comiteamos
                        Update(novedadBase);
                        trx.Commit();
                    }
                    catch (Exception)
                    {
                        trx.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Metodo que guarda la relacion con la novedad para un usuario
        /// </summary>
        /// <param name="novedadId"></param>
        /// <param name="nombreUsuario"></param>
        internal void GuardarNovedadPorUsuario(long novedadId, string nombreUsuario, string comentario)
        {
            //Usuario usuario = GetUsuarioPorNombreUsuario(nombreUsuario);
            //Novedad novedad = GetById<Novedad>(novedadId);
            //NovedadGuardada novedadGuardada = new NovedadGuardada();
            //novedadGuardada.Novedad = novedad;
            //novedadGuardada.Usuario = usuario;
            //novedadGuardada.FechaAlta = DateTime.Now;
            //Insert(novedadGuardada);


            Search search = new Search(typeof(NovedadGuardada));
            search.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            search.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            search.AddExpression(Restrictions.Eq("Usuario.NombreUsuario", nombreUsuario));
            search.AddExpression(Restrictions.Eq("Novedad.Id", novedadId));
            var novedadGuardada = GetByCriteria<NovedadGuardada>(search).FirstOrDefault();
            if (novedadGuardada != null)
            {
                novedadGuardada.FechaActualizacion = DateTime.Now;
                novedadGuardada.FechaBaja = null;
                novedadGuardada.Comentario = comentario;
                Update(novedadGuardada);
            }
            else
            {
                Usuario usuario = GetUsuarioPorNombreUsuario(nombreUsuario);
                Novedad novedad = GetById<Novedad>(novedadId);
                NovedadGuardada novedadSave = new NovedadGuardada();
                novedadSave.Novedad = novedad;
                novedadSave.Usuario = usuario;
                novedadSave.FechaAlta = DateTime.Now;
                novedadSave.FechaBaja = null;
                novedadSave.FechaActualizacion = DateTime.Now;
                novedadSave.Comentario = comentario;
                Insert(novedadSave);
            }

        }

        /// <summary>
        /// Metodo que borra la relacion con la novedad guardada por el usuario
        /// </summary>
        /// <param name="novedadId"></param>
        /// <param name="nombreUsuario"></param>
        internal void BorrarNovedadPorUsuario(long novedadId, string nombreUsuario)
        {
            Search search = new Search(typeof(NovedadGuardada));
            search.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            search.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            search.AddExpression(Restrictions.Eq("Usuario.NombreUsuario", nombreUsuario));
            search.AddExpression(Restrictions.Eq("Novedad.Id", novedadId));
            var novedadGuardada = GetByCriteria<NovedadGuardada>(search).FirstOrDefault();
            if (novedadGuardada != null)
            {
                //Delete<NovedadGuardada>(novedadGuardada.Id);
                novedadGuardada.FechaActualizacion = DateTime.Now;
                novedadGuardada.FechaBaja = DateTime.Now;
                novedadGuardada.Comentario = null;
                Update(novedadGuardada);
            }
        }

        /// <summary>
        /// Metodo que marca como leida a una novedad
        /// </summary>
        /// <param name="novedadId"></param>
        /// <param name="nombreUsuario"></param>
        internal void MarcarNovedadNotificacion(long novedadId, string nombreUsuario)
        {
            Usuario usuario = GetUsuarioPorNombreUsuario(nombreUsuario);

            Novedad novedad = GetById<Novedad>(novedadId);

            NovedadLeida novedadLeida = new NovedadLeida();
            novedadLeida.Novedad = novedad;
            novedadLeida.Usuario = usuario;
            novedadLeida.FechaLectura = DateTime.Now;
            Insert(novedadLeida);
        }
        /// <summary>
        /// Metodo que elimina la novedad seleccionada
        /// </summary>
        /// <param name="id"></param>
        public void EliminarNovedad(long id, int version)
        {
            int versionBase = GetVersion<Novedad>(id);

            //Si el version no coincide elevamos excepcion
            if (versionBase != version)
                throw new Exception("La novedad fue modificada por otro usuario.");

            //Search de novedad leida
            Search searchNovedadLeida = new Search(typeof(NovedadLeida));
            searchNovedadLeida.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            searchNovedadLeida.AddExpression(Restrictions.Eq("Novedad.Id", id));
            IList<NovedadLeida> novedadLeida = GetByCriteria<NovedadLeida>(searchNovedadLeida, new string[] { "Id" });

            //Search de novedad guardadas
            Search searchNovedadGuardada = new Search(typeof(NovedadGuardada));
            searchNovedadGuardada.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            searchNovedadGuardada.AddExpression(Restrictions.Eq("Novedad.Id", id));
            IList<NovedadGuardada> novedadGuardada = GetByCriteria<NovedadGuardada>(searchNovedadGuardada, new string[] { "Id" });

            //Search de novedad rol nodo
            Search searchNovedadRolNodo = new Search(typeof(NovedadRolNodo));
            searchNovedadRolNodo.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            searchNovedadRolNodo.AddExpression(Restrictions.Eq("Novedad.Id", id));
            IList<NovedadRolNodo> novedadRolNodos = GetByCriteria<NovedadRolNodo>(searchNovedadRolNodo, new string[] { "Id" });

            using (var trx = BeginTransaction())
            {
                try
                {
                    //Si existe novedades leidas asociados a la novedad las borro
                    if (novedadLeida.Count > 0)
                        DeleteAll<NovedadLeida>(novedadLeida.Select(x => x.GetId()).ToList(), false);

                    //Si existe novedades guardadas asociados a la novedad las borro
                    if (novedadGuardada.Count > 0)
                        DeleteAll<NovedadGuardada>(novedadGuardada.Select(x => x.GetId()).ToList(), false);

                    //Si existe novedades rol nodo asociados a la novedad las borro
                    if (novedadRolNodos.Count > 0)
                        DeleteAll<NovedadRolNodo>(novedadRolNodos.Select(x => x.GetId()).ToList(), false);

                    Delete<Novedad>(id);
                    trx.Commit();
                }
                catch (Exception)
                {
                    trx.Rollback();
                    throw;
                }
            }
            //Delete<Novedad>(id);
        }

        /// <summary>
        /// Metodo que retorna una novedad segun Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NovedadDTO GetNovedadEditar(long id)
        {
            Novedad novedad = GetById<Novedad>(id);
            NovedadDTO novedadEditar = novedad.Adapt<NovedadDTO>();
            return novedadEditar;
        }

        #region // Metodos para la api de novedades //

        /// <summary>
        /// Metodo que obtiene las novedades no leidas por el usuario segun el tipo de novedad
        /// </summary>
        /// <param name="username">Usuario</param>
        /// <param name="tipoNovedad">NOTIFICACION o ADVERTENCIA</param>
        /// <returns></returns>
        internal List<NovedadDTO> GetNovedadesNoLeidas(string username, string tipoNovedad)
        {
            List<NovedadDTO> novedadesTodas = GetTodasNovedades(username, tipoNovedad);
            List<NovedadDTO> novedadesLeidas = GetNovedadesLeidas(username);
            if (novedadesLeidas != null)
            {
                List<NovedadDTO> resultado = novedadesTodas.Where(x => !novedadesLeidas.Select(n => n.Id).ToList().Contains(x.Id)).OrderByDescending(z => z.Id).ToList();
                return resultado;
            }
            else
                return novedadesTodas;
        }

        /// <summary>
        /// Metodo que recupera las noveades guardadas del usuario
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <returns></returns>
        internal List<NovedadDTO> GetNovedadesGuardadas(string nombreUsuario)
        {
            List<NovedadDTO> novedadesGuardadas = SearchNovedadesGuardadas(nombreUsuario);
            return novedadesGuardadas;
        }

        /// <summary>
        /// Metodo que obtiene las novedades de tipo advertencia del usuario
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <returns></returns>
        internal List<NovedadDTO> GetAdvertenciasTotales(string nombreUsuario)
        {
            return GetNovedadesTotales(nombreUsuario, TipoNovedadConstants.TIPO_NOVEDAD_ADVERTENCIA);
        }

        /// <summary>
        /// Metodo que obtiene las novedades dle usuario y por tipo
        /// </summary>
        /// <param name="username"></param>
        /// <param name="tipoNovedad"></param>
        /// <returns></returns>
        internal List<NovedadDTO> GetNovedadesTotales(string username, string tipoNovedad)
        {
            List<NovedadDTO> novedadesTodas = GetTodasNovedades(username, tipoNovedad);
            List<NovedadDTO> novedadesLeidas = GetNovedadesLeidas(username);
            if (novedadesLeidas != null)
            {
                foreach (NovedadDTO novedad in novedadesTodas)
                {
                    if (novedadesLeidas.Select(n => n.Id).ToList().Contains(novedad.Id))
                        novedad.SiLeida = true;
                }
            }
            return novedadesTodas;
        }

        /// <summary>
        /// Metodo que obtiene las novedades leidas por el usuario
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private List<NovedadDTO> GetNovedadesLeidas(string username)
        {
            Search searchRelacion = CrearSearchNovedadesLeidas(username);
            var relaciones = GetByCriteria<NovedadLeida>(searchRelacion);
            List<NovedadLeida> listaRelaciones = (relaciones != null) ? relaciones.Cast<NovedadLeida>().ToList() : new List<NovedadLeida>();
            var novedadesSelect = listaRelaciones.Select(a => a.Novedad).ToList<Novedad>().Distinct().ToList();
            if (novedadesSelect.Count > 0)
                return novedadesSelect.Adapt<IList<NovedadDTO>>().ToList();
            else
                return null;
        }

        private List<NovedadDTO> SearchNovedadesGuardadas(string nombreUsuario)
        {
            Search searchRelacion = CrearSearchGuardada(nombreUsuario);
            var relaciones = GetByCriteria<NovedadGuardada>(searchRelacion);
            List<NovedadGuardada> listaRelaciones = (relaciones != null) ? relaciones.Cast<NovedadGuardada>().ToList() : new List<NovedadGuardada>();
            var novedadesSelect = listaRelaciones.Select(a => a.Novedad).ToList<Novedad>().Distinct().ToList();
            if (novedadesSelect.Count > 0)
                return novedadesSelect.Adapt<IList<NovedadDTO>>().ToList();
            else
                return null;
        }

        /// <summary>
        /// Metodo que recupera todas la novedades del usuario y tipo de novedad
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="tipoNovedad"></param>
        /// <returns></returns>
        private List<NovedadDTO> GetTodasNovedades(string nombreUsuario, string tipoNovedad)
        {
            var publicas = GetNovedadesPublicas(tipoNovedad);
            var privadas = GetNovedadesPrivadas(nombreUsuario, tipoNovedad);
            if (privadas != null)
                return publicas.Concat(privadas).OrderByDescending(n => n.Id).ToList();
            else
                return publicas;
        }

        /// <summary>
        /// Metodo que obtiene las novedades privadas del usuario y tipo de novedad
        /// </summary>
        /// <param name="nombreUsuario"></param>
        /// <param name="tipoNovedad"></param>
        /// <returns></returns>
        private List<NovedadDTO> GetNovedadesPrivadas(string nombreUsuario, string tipoNovedad)
        {
            List<NovedadDTO> novedades = new List<NovedadDTO>();
            //string[] propiedadesNovedad = new string[]
            //   {
            //        "Id",
            //        "Version",
            //        "SiPublica",
            //        "Descripcion",
            //        "TipoNovedad",
            //        "FechaDesde",
            //        "FechaHasta",
            //        "NumeroVersion",
            //        "EstructuraFuncional.Codigo"
            //   };
            //busco los roles del usuario 
            //recupero los roles y a partir de los roles recupero el rol nodo usuaio para obtener los nodos

            long[] roles = GetRoles(nombreUsuario);
            List<RolNodoUsuario> rolNodoUsuario = GetNodosPorRol(roles, nombreUsuario);
            long[] nodos = rolNodoUsuario.Select(n => n.NodoFuncional.Id).ToArray();
            //genero la busqueda de novedades para esos roles
            Search searchRelacion = CrearSearchNovedades(roles, tipoNovedad, nodos);
            try
            {
                var relaciones = GetByCriteria<NovedadRolNodo>(searchRelacion);
                List<NovedadRolNodo> listaRelaciones = (relaciones != null) ? relaciones.Cast<NovedadRolNodo>().ToList() : new List<NovedadRolNodo>();
                var novedadesSelect = listaRelaciones.Select(a => a.Novedad).ToList().Distinct().ToList();
                if (novedadesSelect.Count > 0)
                    return novedadesSelect.Adapt<IList<NovedadDTO>>().ToList();
                else
                    return null;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Metodo que recupera los id de los roles de un usuario
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private List<RolNodoUsuario> GetNodosPorRol(long[] roles, string nombreUsuario)
        {

            Search search = new Search(typeof(RolNodoUsuario));
            search.AddAlias(new KeyValuePair<string, string>("Rol", "Rol"));
            search.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            search.AddExpression(Restrictions.In("Rol.Id", roles));
            search.AddExpression(Expression.Eq("Usuario.NombreUsuario", nombreUsuario));
            var rolesFiltrados = GetByCriteria<RolNodoUsuario>(search);
            List<RolNodoUsuario> rnu = (rolesFiltrados != null) ? rolesFiltrados.Cast<RolNodoUsuario>().ToList() : new List<RolNodoUsuario>();
            return rnu;
        }

        /// <summary>
        /// Metodo que recupera los id de los roles de un usuario
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private long[] GetRoles(string username)
        {
            string[] propiedades = new string[]
               {
                   "IdEstructuraFuncional",
                   "DescripcionNodoFuncional",
                   "DesripcionActividad",
                    "Usuario",
                    "CodigoEstructura",
                    "Rol.Id"
               };
            Search searchRoles = CrearSearchRoles(username);
            var rolesFiltrados = GetByCriteria<VWUsuarioPermisos>(searchRoles, propiedades);
            List<VWUsuarioPermisos> listaRoles = (rolesFiltrados != null) ? rolesFiltrados.Cast<VWUsuarioPermisos>().ToList() : new List<VWUsuarioPermisos>();
            return listaRoles.Select(vw => vw.Rol.Id).Distinct().ToArray();
        }

        /// <summary>
        /// Metodo que recupera las novedades publicas por tipo de novedad
        /// </summary>
        /// <param name="tipoNovedad"></param>
        /// <returns></returns>
        private List<NovedadDTO> GetNovedadesPublicas(string tipoNovedad)
        {
            List<NovedadDTO> novedades = new List<NovedadDTO>();
            Search search = CrearSearchNovedadesPublicas(tipoNovedad);
            IList<Novedad> novedadesPublicas = GetByCriteria<Novedad>(search);
            IList<NovedadDTO> novedadesPublicasViewModel = novedadesPublicas.Adapt<IList<NovedadDTO>>();
            if (novedadesPublicas != null && novedadesPublicas.Count > 0)
                ((List<NovedadDTO>)novedades).AddRange(novedadesPublicasViewModel);
            return novedades;
        }

        /// <summary>
        /// Metodo que determina si una novedad es leida o no
        /// </summary>
        /// <param name="novedadId"></param>
        /// <param name="nombreUsuario"></param>
        /// <returns></returns>
        internal bool EsNovedadLeida(long novedadId, string nombreUsuario)
        {
            Search search = CrearSearchEsNovedadesLeida(novedadId, nombreUsuario);
            var relaciones = GetByCriteria<NovedadLeida>(search);
            return relaciones.Count > 0;
        }

        /// <summary>
        /// Metodo que verifica si la novedad ya fue guardada para un usuario
        /// </summary>
        /// <param name="novedadId"></param>
        /// <param name="nombreUsuario"></param>
        /// <returns></returns>
        internal bool SiNovedadGuardada(long novedadId, string nombreUsuario)
        {
            Search search = new Search(typeof(NovedadGuardada));
            search.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            search.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            search.AddExpression(Restrictions.Eq("Usuario.NombreUsuario", nombreUsuario));
            search.AddExpression(Restrictions.Eq("Novedad.Id", novedadId));
            search.AddExpression(Restrictions.IsNull("FechaBaja"));
            var relaciones = GetByCriteria<NovedadGuardada>(search);
            return relaciones.Count > 0;
        }

        #region //  Searchs  //

        /// <summary>
        /// Metodo que arme el search para saber si la novedad es leida o no
        /// </summary>
        /// <param name="idNovedad"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal static Search CrearSearchEsNovedadesLeida(long idNovedad, string username)
        {
            Search search = new Search(typeof(NovedadLeida));
            search.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            search.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            search.AddExpression(Restrictions.Eq("Usuario.NombreUsuario", username));
            search.AddExpression(Restrictions.Eq("Novedad.Id", idNovedad));
            return search;
        }
        /// <summary>
        /// Metodo que creea el search para las novedades leidas del usuario
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private Search CrearSearchNovedadesLeidas(string username)
        {
            Search search = new Search(typeof(NovedadLeida));
            search.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            search.AddExpression(Restrictions.Eq("Usuario.NombreUsuario", username));
            return search;
        }
        private Search CrearSearchGuardada(string nombreUsuario)
        {
            Search search = new Search(typeof(NovedadGuardada));
            search.AddAlias(new KeyValuePair<string, string>("Usuario", "Usuario"));
            search.AddExpression(Restrictions.Eq("Usuario.NombreUsuario", nombreUsuario));
            search.AddExpression(Restrictions.IsNull("FechaBaja"));

            return search;
        }
        /// <summary>
        /// Metodo que crea el search de las noveades de una lista de roles y tipo de novedad
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="tipoNovedad"></param>
        /// <returns></returns>
        private Search CrearSearchNovedades(long[] roles, string tipoNovedad, long[] nodos)
        {
            Search search = new Search(typeof(NovedadRolNodoDTO));
            search.AddAlias(new KeyValuePair<string, string>("NodoFuncional", "Nodo"));

            search.AddExpression(Restrictions.In("Rol.Id", roles));
            search.AddExpression(Restrictions.In("Nodo.Id", nodos));

            search.AddAlias(new KeyValuePair<string, string>("Novedad", "Novedad"));
            search.AddExpression(Restrictions.Eq("Novedad.TipoNovedad", tipoNovedad));

            DateTime diaActual = DateTime.Now;
            diaActual.ToString("yyyyMMddHHmmssffff");
            search.AddExpression(Restrictions.Le("Novedad.FechaDesde", diaActual));
            //search.AddExpression(Restrictions.Disjunction().Add(Restrictions.IsNull("Novedad.FechaHasta")).Add(Restrictions.Ge("Novedad.FechaHasta", HelperBrokerService.Instance.GetDateTodayStartDay())));
            //search.AddExpression(Restrictions.Disjunction().Add(Restrictions.IsNull("Novedad.FechaHasta")).Add(Restrictions.Ge("Novedad.FechaHasta", diaActual)));

            return search;
        }

        /// <summary>
        /// Metodo que crea el search para recuperar los roles de un usuario
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private Search CrearSearchRoles(string username)
        {
            Search search = new Search(typeof(VWUsuarioPermisosDTO));
            ICriterion expressionUsuario = Restrictions.Eq("Usuario", username);
            search.AddExpression(expressionUsuario);
            return search;
        }
        /// <summary>
        /// Metodo que crea el search para recuperar las novedades publicas por tipo de novedad
        /// </summary>
        /// <param name="tipoNovedad"></param>
        /// <returns></returns>
        private Search CrearSearchNovedadesPublicas(string tipoNovedad)
        {
            Search search = new Search(typeof(NovedadDTO));
            search.AddExpression(Restrictions.Eq("SiPublica", true));
            search.AddExpression(Restrictions.Eq("TipoNovedad", tipoNovedad));
            //search.AddExpression(Restrictions.Le("FechaDesde", HelperBrokerService.Instance.GetDateTodayStartDay().Date));
            DateTime diaActual = DateTime.Now;
            diaActual.ToString("yyyyMMddHHmmssffff");
            search.AddExpression(Restrictions.Le("FechaDesde", diaActual));
            //search.AddExpression(Restrictions.Disjunction().Add(Restrictions.IsNull("FechaHasta")).Add(Restrictions.Ge("FechaHasta", HelperBrokerService.Instance.GetDateTodayStartDay())));
            if (tipoNovedad == TipoNovedadConstants.TIPO_NOVEDAD_ADVERTENCIA)
                search.AddExpression(Restrictions.Disjunction().Add(Restrictions.IsNull("FechaHasta")).Add(Restrictions.Ge("FechaHasta", diaActual)));
            return search;
        }
        #endregion
        #endregion
    }

}
