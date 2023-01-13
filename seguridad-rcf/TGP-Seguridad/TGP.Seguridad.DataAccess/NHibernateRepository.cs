//using AuditoriaLibrary.Configuration;
using FluentNHibernate;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGP.Seguridad.DataAccess.Infrastructure;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Infrastructure.Transactions;
using TGP.Seguridad.DataAccess.Helpers;

namespace TGP.Seguridad.DataAccess
{
    /// <summary>
    /// Implementacion generica de la capa de acceso a datos (Repositorio).
    /// Esta clase puede ser extendida por para hacer repositorios especificos para una clase.
    /// Se gestiona la session y transacciones de NHibernate a travez del NHibernateHelper
    /// Ademas posee los servicios CRUD basicos.
    /// </summary>
    public class NHibernateRepository
    {
        #region // Propiedades Publicas //
        /// <summary>
        /// Session de NHibernate
        /// </summary>
        public ISession Session
        {
            get
            {
                ISession session = NHibernateHelper.GetCurrentSession(SessionWrapper.GetUser());
                // configuramos la liberia AuditoriaLibrary para que tenga la session de NHibernate en su contexto.
                //ConfigurationHelper.ConfigurarLibrary(session);
                return session;
            }
        }
        /// <summary>
        /// Encapsula la transaccion que esta en _trx
        /// </summary>
        public Transaction Transaction
        {
            get { return this._trx; }
            set { this._trx = value; }
        }
        #endregion

        #region // Propiedades Privadas //

        /// <summary>
        /// Wrapper que encapsula la utilizacion de las transacciones
        /// </summary>
        private Transaction _trx;

        private static NHibernateRepository instance;

        public static NHibernateRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NHibernateRepository();
                }
                return instance;
            }
        }

        #endregion

        private NHibernateRepository()
        { }

        #region // IRepository //

        public ISession GetSessionProConfig()
        {
            return null;
        }

        #region // Criterias //
        /// <summary>
        /// Gets a Criteria which excludes the phisically or logically deleted records
        /// </summary>
        /// <returns>ICriteria</returns>
        protected ICriteria GetCriteria(System.Type persistent, ISession altSession = null)
        {
            if (altSession != null)
                return altSession.CreateCriteria(persistent, "Criteria");
            else
                return this.Session.CreateCriteria(persistent, "Criteria");
        }



        /// <summary>
        /// Gets a Criteria which excludes the phisically or logically deleted records and applies a count projection
        /// </summary>
        /// <returns>ICriteria</returns>
        protected ICriteria GetCountCriteria(System.Type persistent)
        {
            return this.GetCriteria(persistent).SetProjection(Projections.RowCount());
        }

        /// <summary>
        /// Gets a Criteria which excludes the phisically or logically deleted records and applies a count projection
        /// </summary>
        /// <returns>ICriteria</returns>
        protected ICriteria GetCountCriteria(ICriteria criteria)
        {
            return criteria.SetProjection(Projections.RowCount());
        }

        ///// <summary>
        ///// Gets a paged Criteria which excludes the phisically or logically deleted records and applies a count projection
        ///// </summary>
        ///// <param name="pager">Paging options</param>
        ///// <returns>ICriteria</returns>
        //protected ICriteria GetCountCriteria(Pager pager, System.Type persistent)
        //{
        //    return this.GetCriteria(pager, persistent).SetProjection(Projections.RowCount());
        //}
        #endregion

        #region // Metodos Publicos //

        #region GetAll
        /// <summary>
        /// Devuelve un Response con una lista de todos los objetos tipados en T de la base de datos
        /// </summary>
        /// <returns></returns>
        public IList<BaseEntity> GetAll(System.Type persistent, ISession session = null)
        {

            return this.GetCriteria(persistent, session).List<BaseEntity>();
        }

        /// <summary>
        /// Devuelve una lista de todos los objetos tipados en T de la base de datos
        /// </summary>
        /// <returns></returns>
        public IList<T> GetAll<T>(ISession session = null)
        {
            return this.GetCriteria(typeof(T), session).List<T>();
        }

        /// <summary>
        /// Devuelve un Response con una lista de todos los objetos tipados en T de la base de datos
        /// </summary>
        /// <returns></returns>
        public IList<T> GetAll<T>(string[] propiedades, ISession session = null) where T : class
        {

            ICriteria criteria = this.GetCriteria(typeof(T), session);

            //Lista de columnas o propiedades que queremos traer
            var projections = Projections.ProjectionList();

            CreateProjection(propiedades, criteria);


            ICriteria criteriaCount = (ICriteria)criteria.Clone();
            //Count de la consulta
            IFutureValue<int> count = this.GetCountCriteria(criteriaCount).FutureValue<int>();

            //Usamos el deeptransformer para convertir las tuplas en entidades
            criteria.SetResultTransformer(new DeepTransformer(propiedades, typeof(T)));

            return criteria.List<T>();
        }

        /// <summary>
        /// Devuelve un Response con una lista de todos los objetos tipados en T de la base de datos
        /// </summary>
        /// <returns></returns>
        public IList<BaseEntity> GetAll(System.Type persistent, string[] propiedades, ISession session = null)
        {

            ICriteria criteria = this.GetCriteria(persistent, session);

            //Lista de columnas o propiedades que queremos traer
            var projections = Projections.ProjectionList();

            CreateProjection(propiedades, criteria);


            ICriteria criteriaCount = (ICriteria)criteria.Clone();
            //Count de la consulta
            IFutureValue<int> count = this.GetCountCriteria(criteriaCount).FutureValue<int>();

            //Usamos el deeptransformer para convertir las tuplas en entidades
            criteria.SetResultTransformer(new DeepTransformer(propiedades, persistent));

            return criteria.List<BaseEntity>();
        }
        #endregion

        /// <summary>
        /// Metodo que crea una projection son las propiedades que se pasan como parametro
        /// </summary>
        /// <param name="propiedades"></param>
        /// <param name="criteria"></param>
        private static void CreateProjection(string[] propiedades, ICriteria criteria)
        {
            //Lista de alias a crear
            List<string> aliases = new List<string>();

            //Lista de columnas o propiedades que queremos traer
            var projections = Projections.ProjectionList();
            foreach (string property in propiedades)
            {
                //Si la descripcion de la propiedad contiene '.' es compuesta.
                if (property.Contains("."))
                {
                    //Nos quedamos con el ultimo par 'Entidad.Propiedad'
                    string[] propertySplit = property.Split('.');
                    for (int i = 0; i < propertySplit.Length - 1; i++)
                    {
                        //Si ya existe el alias no lo agregamos de nuevo
                        if (!aliases.Any(x => x.Equals(propertySplit[i])))
                        {
                            //Si es el indice es 0 significa que es propiedad de la entidad base
                            if (i == 0)
                                criteria.CreateAlias(propertySplit[i], propertySplit[i], NHibernate.SqlCommand.JoinType.LeftOuterJoin);
                            //Sino debe provenir de otro alias, por eso indicamos i-1 en el primer parametro de createalias
                            else
                                criteria.CreateAlias(propertySplit[i - 1] + "." + propertySplit[i], propertySplit[i], NHibernate.SqlCommand.JoinType.LeftOuterJoin);
                            //finalmente agregamos el alias a nuestra coleccion para verificar que ya lo creamos
                            aliases.Add(propertySplit[i]);
                        }
                    }
                    string p = propertySplit[propertySplit.Length - 2] + "." + propertySplit[propertySplit.Length - 1];
                    //Lo agregamos a la lista de projections
                    projections.Add(Projections.Property(p), p);
                }
                else
                {
                    projections.Add(Projections.Property(property), property);
                }
            }


            criteria.SetProjection(projections);
        }

        #region GetByCriteria
        /// <summary>
        /// Devuelve un Response con una lista de todos los objetos tipados en T de la base de datos segun el criteria recibido
        /// </summary>
        /// <returns></returns>
        public IList<T> GetByCriteria<T>(ICriteria criteria) where T : class
        {

            return criteria.List<T>();
        }

        /// <summary>
        /// Devuelve un Response con una lista de todos los objetos tipados en T de la base de datos segun el criteria recibido
        /// </summary>
        /// <returns></returns>
        public IList<BaseEntity> GetByCriteria(ICriteria criteria)
        {

            return criteria.List<BaseEntity>();
        }

        /// <summary>
        /// Devuelve un Response con una lista de todos los objetos tipados en T de la base de datos segun el criteria recibido
        /// y las propiedades requeridas
        /// </summary>
        /// <returns></returns>
        public IList<BaseEntity> GetByCriteria(ICriteria criteria, string[] propiedades, Type tipoEntidad, bool distinct = false, IList<string> aliasesSearch = null)
        {
            //Lista de columnas o propiedades que queremos traer
            var projections = Projections.ProjectionList();

            foreach (string property in propiedades)
            {
                //Si la descripcion de la propiedad contiene '.' es compuesta.
                if (property.Contains("."))
                {
                    //Nos quedamos con el ultimo par 'Entidad.Propiedad'
                    string[] propertySplit = property.Split('.');
                    for (int i = 0; i < propertySplit.Length - 1; i++)
                    {
                        //Si ya existe el alias no lo agregamos de nuevo
                        if (!aliasesSearch.Any(x => x.Equals(propertySplit[i])))
                        {
                            //Si es el indice es 0 significa que es propiedad de la entidad base
                            if (i == 0)
                                criteria.CreateAlias(propertySplit[i], propertySplit[i], NHibernate.SqlCommand.JoinType.LeftOuterJoin);
                            //Sino debe provenir de otro alias, por eso indicamos i-1 en el primer parametro de createalias
                            else
                                criteria.CreateAlias(propertySplit[i - 1] + "." + propertySplit[i], propertySplit[i], NHibernate.SqlCommand.JoinType.LeftOuterJoin);
                            //finalmente agregamos el alias a nuestra coleccion para verificar que ya lo creamos
                            aliasesSearch.Add(propertySplit[i]);
                        }
                    }
                    string p = propertySplit[propertySplit.Length - 2] + "." + propertySplit[propertySplit.Length - 1];
                    //Lo agregamos a la lista de projections
                    projections.Add(Projections.Property(p), p);
                }
                else
                {
                    projections.Add(Projections.Property(property), property);
                }
            }

            if (distinct)
                criteria.SetProjection(Projections.Distinct(projections));
            else
                criteria.SetProjection(projections);


            //ICriteria criteriaCount = (ICriteria)criteria.Clone();
            //Count de la consulta
            //IFutureValue<int> count = this.GetCountCriteria(criteriaCount).FutureValue<int>();

            //Usamos el deeptransformer para convertir las tuplas en entidades
            criteria.SetResultTransformer(new DeepTransformer(propiedades, tipoEntidad));
            return criteria.List<BaseEntity>();
        }

        /// <summary>
        /// Devuelve un Response con una lista de todos los objetos tipados en T de la base de datos segun el criteria recibido
        /// y las propiedades requeridas
        /// </summary>
        /// <returns></returns>
        public IList<T> GetByCriteria<T>(ICriteria criteria, string[] propiedades, bool distinct = false, IList<string> aliasesSearch = null) where T : class
        {
            //Lista de columnas o propiedades que queremos traer
            var projections = Projections.ProjectionList();

            foreach (string property in propiedades)
            {
                //Si la descripcion de la propiedad contiene '.' es compuesta.
                if (property.Contains("."))
                {
                    //Nos quedamos con el ultimo par 'Entidad.Propiedad'
                    string[] propertySplit = property.Split('.');
                    for (int i = 0; i < propertySplit.Length - 1; i++)
                    {
                        //Si ya existe el alias no lo agregamos de nuevo
                        if (!aliasesSearch.Any(x => x.Equals(propertySplit[i])))
                        {
                            //Si es el indice es 0 significa que es propiedad de la entidad base
                            if (i == 0)
                                criteria.CreateAlias(propertySplit[i], propertySplit[i], NHibernate.SqlCommand.JoinType.LeftOuterJoin);
                            //Sino debe provenir de otro alias, por eso indicamos i-1 en el primer parametro de createalias
                            else
                                criteria.CreateAlias(propertySplit[i - 1] + "." + propertySplit[i], propertySplit[i], NHibernate.SqlCommand.JoinType.LeftOuterJoin);
                            //finalmente agregamos el alias a nuestra coleccion para verificar que ya lo creamos
                            aliasesSearch.Add(propertySplit[i]);
                        }
                    }
                    string p = propertySplit[propertySplit.Length - 2] + "." + propertySplit[propertySplit.Length - 1];
                    //Lo agregamos a la lista de projections
                    projections.Add(Projections.Property(p), p);
                }
                else
                {
                    projections.Add(Projections.Property(property), property);
                }
            }

            if (distinct)
                criteria.SetProjection(Projections.Distinct(projections));
            else
                criteria.SetProjection(projections);


            //ICriteria criteriaCount = (ICriteria)criteria.Clone();
            //Count de la consulta
            //IFutureValue<int> count = this.GetCountCriteria(criteriaCount).FutureValue<int>();

            //Usamos el deeptransformer para convertir las tuplas en entidades
            criteria.SetResultTransformer(new DeepTransformer(propiedades, typeof(T)));
            return criteria.List<T>();
        }
        #endregion

        /// <summary>
        /// Metodo que devuelve el count del criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public int GetCountByCriteria(ICriteria criteria)
        {
            ICriteria criteriaCount = (ICriteria)criteria.Clone();
            return (int)criteriaCount.SetProjection(Projections.RowCount()).UniqueResult();
        }

        /// <summary>
        /// Devuelve un Response con una lista de todos los objetos tipados en T de la 
        /// base de datos con filtro ingresado
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IList<BaseEntity> Search(System.Type filter)
        {
            return this.Search(this.GetCriteria(filter), filter);
        }

        #region GetById

        /// <summary>
        /// Metodo para ir a buscar un objeto a la base por id, sin importar si es Id o compisiteId.
        /// </summary>
        /// <param name="ids">diccionario con el conjuntos de ids y sus valores para ir a buscar un objeto a la base</param>
        /// <param name="baseEntityType">tipo de la entidad persistente a buscar</param>
        /// <returns>la entidad encontrada</returns>
        public BaseEntity GetById(Dictionary<string, object> ids, Type baseEntityType, ISession altSession = null)
        {
            //creamos un criteria
            ICriteria criteria;
            if (altSession != null)
                criteria = altSession.CreateCriteria(baseEntityType, "Criteria");
            else
                criteria = Session.CreateCriteria(baseEntityType, "Criteria");
            //iteramos sobre las ids del objeto a buscar
            foreach (KeyValuePair<string, object> id in ids)
            {
                //agregamos condicion cuyo campo es la key y el valor sale del value.
                criteria.Add(Expression.Eq(id.Key, id.Value));
            }
            BaseEntity elemento = (BaseEntity)criteria.UniqueResult<BaseEntity>();
            //retornamos el unico elemento
            return elemento;
        }

        /// <summary>
        /// Metodo para ir a buscar un objeto a la base por id, sin importar si es Id o compisiteId.
        /// </summary>
        /// <param name="ids">diccionario con el conjuntos de ids y sus valores para ir a buscar un objeto a la base</param>
        /// <returns>la entidad encontrada</returns>
        public T GetById<T>(Dictionary<string, object> ids, ISession altSession = null) where T : class
        {
            //creamos un criteria
            ICriteria criteria;
            if (altSession != null)
                criteria = altSession.CreateCriteria(typeof(T), "Criteria");
            else
                criteria = Session.CreateCriteria(typeof(T), "Criteria");
            //iteramos sobre las ids del objeto a buscar
            foreach (KeyValuePair<string, object> id in ids)
            {
                //agregamos condicion cuyo campo es la key y el valor sale del value.
                criteria.Add(Expression.Eq(id.Key, id.Value));
            }
            //retornamos el unico elemento
            return criteria.UniqueResult<T>();
        }

        /// <summary>
        /// Retorna un objeto persistido tipado con T cuyo id viene por parametro.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById<T>(long id, ISession altSession = null) where T : class
        {

            try
            {
                if (altSession != null)
                    return altSession.Get(typeof(T), id) as T;
                else
                    return Session.Get(typeof(T), id) as T;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Retorna un objeto persistido tipado con T cuyo id viene por parametro.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseEntity GetById(long id, Type baseEntityType, ISession altSession = null)
        {
            if (altSession != null)
                return (BaseEntity)altSession.Get(baseEntityType, id);
            else
                return (BaseEntity)Session.Get(baseEntityType, id);
        }


        /// <summary>
        /// Retorna un objeto persistido tipado con T cuyo id viene por parametro.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById<T>(long id, string[] propiedades, ISession altSession = null) where T : class
        {
            try
            {
                //creamos un criteria
                ICriteria criteria;
                if (altSession != null)
                    criteria = altSession.CreateCriteria(typeof(T), "Criteria");
                else
                    criteria = Session.CreateCriteria(typeof(T), "Criteria");
                criteria.Add(Expression.Eq("Id", id));
                CreateProjection(propiedades, criteria);
                ICriteria criteriaCount = (ICriteria)criteria.Clone();

                //Count de la consulta
                //IFutureValue<int> count = this.GetCountCriteria(criteriaCount).FutureValue<int>();

                //Usamos el deeptransformer para convertir las tuplas en entidades
                criteria.SetResultTransformer(new DeepTransformer(propiedades, typeof(T)));

                return criteria.UniqueResult<T>();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        /// <summary>
        /// Retorna un objeto persistido tipado con T cuyo id viene por parametro.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseEntity GetById(long id, Type baseEntityType, string[] propiedades, ISession altSession = null)
        {
            try
            {
                //creamos un criteria
                ICriteria criteria; 
                if(altSession != null)
                    criteria = altSession.CreateCriteria(baseEntityType, "Criteria");
                else
                    criteria = Session.CreateCriteria(baseEntityType, "Criteria");
                criteria.Add(Expression.Eq("Id", id));
                CreateProjection(propiedades, criteria);
                ICriteria criteriaCount = (ICriteria)criteria.Clone();

                //Count de la consulta
                //IFutureValue<int> count = this.GetCountCriteria(criteriaCount).FutureValue<int>();

                //Usamos el deeptransformer para convertir las tuplas en entidades
                criteria.SetResultTransformer(new DeepTransformer(propiedades, baseEntityType));

                return criteria.UniqueResult<BaseEntity>();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        /// <summary>
        /// Metodo para ir a buscar un objeto a la base por id, sin importar si es Id o compisiteId.
        /// </summary>
        /// <param name="ids">diccionario con el conjuntos de ids y sus valores para ir a buscar un objeto a la base</param>
        /// <param name="baseEntityType">tipo de la entidad persistente a buscar</param>
        /// <returns>la entidad encontrada</returns>
        public BaseEntity GetById(Dictionary<string, object> ids, Type baseEntityType, string[] propiedades, ISession altSession = null)
        {
            //creamos un criteria
            ICriteria criteria;
            if (altSession != null)
                criteria = altSession.CreateCriteria(baseEntityType, "Criteria");
            else
                criteria = Session.CreateCriteria(baseEntityType, "Criteria");
            //iteramos sobre las ids del objeto a buscar
            foreach (KeyValuePair<string, object> id in ids)
            {
                //agregamos condicion cuyo campo es la key y el valor sale del value.
                criteria.Add(Expression.Eq(id.Key, id.Value));
            }

            CreateProjection(propiedades, criteria);

            ICriteria criteriaCount = (ICriteria)criteria.Clone();
            //Count de la consulta
            IFutureValue<int> count = this.GetCountCriteria(criteriaCount).FutureValue<int>();

            //Usamos el deeptransformer para convertir las tuplas en entidades
            criteria.SetResultTransformer(new DeepTransformer(propiedades, baseEntityType));

            BaseEntity elemento = (BaseEntity)criteria.UniqueResult();
            //retornamos el unico elemento
            return elemento;
        }

        /// <summary>
        /// Metodo para ir a buscar un objeto a la base por id, sin importar si es Id o compisiteId.
        /// </summary>
        /// <param name="ids">diccionario con el conjuntos de ids y sus valores para ir a buscar un objeto a la base</param>
        /// <param name="baseEntityType">tipo de la entidad persistente a buscar</param>
        /// <returns>la entidad encontrada</returns>
        public T GetById<T>(Dictionary<string, object> ids, string[] propiedades, ISession altSession = null) where T : class
        {
            //creamos un criteria
            ICriteria criteria;
            if (altSession != null)
                criteria = altSession.CreateCriteria(typeof(T), "Criteria");
            else
                criteria = Session.CreateCriteria(typeof(T), "Criteria");
            //iteramos sobre las ids del objeto a buscar
            foreach (KeyValuePair<string, object> id in ids)
            {
                //agregamos condicion cuyo campo es la key y el valor sale del value.
                criteria.Add(Expression.Eq(id.Key, id.Value));
            }

            CreateProjection(propiedades, criteria);

            //ICriteria criteriaCount = (ICriteria)criteria.Clone();
            //Count de la consulta
            //IFutureValue<int> count = this.GetCountCriteria(criteriaCount).FutureValue<int>();

            //Usamos el deeptransformer para convertir las tuplas en entidades
            criteria.SetResultTransformer(new DeepTransformer(propiedades, typeof(T)));

            return criteria.UniqueResult<T>();

        }
        #endregion



        /// <summary>
        /// Elimina un objeto, tipado en T, persistido en la BBDD
        /// </summary>
        /// <param name="t"></param>
        public void Delete(BaseEntity t, ISession altSession = null)
        {
            ISession session = altSession ?? this.Session;

            if (session.Transaction != null && session.Transaction.IsActive)
            {
                session.Delete(t);
                session.Flush();
            }
            else
            {
                using (var trx = session.BeginTransaction())
                {
                    session.Delete(t);
                    trx.Commit();
                }
            }
        }

        /// <summary>
        /// Actualiza un objeto, tipado en T, persistido en la BBDD
        /// </summary>
        /// <param name="t"></param>
        public void Update(BaseEntity t, ISession altSession = null)
        {
            ISession session = altSession ?? this.Session;

            if (session.Transaction != null && session.Transaction.IsActive)
            {
                if (HttpContext.Current.Session != null)
                    // seteamos en el contexto de la DB con un SP el usuario que hace la operacion - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    //UserDBContextSPFactory.SetUserToDBContext(session, HttpContext.Current.Session["usuario"].ToString());

                session.Update(t);
                // hacemos flush para que la entidad tenga seteado el usuario - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                session.Flush();

                //if (HttpContext.Current.Session != null)
                    // Limpiamos con un SP el usuario seteado en el contexto de la BD - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    //UserDBContextSPFactory.CleanUserToDBContext(session);
            }
            else
            {
                using (var trx = session.BeginTransaction())
                {
                    session.Update(t);
                    trx.Commit();
                }
            }
        }

        /// <summary>
        /// Inserta un objeto, tipado en T, en la BBDD
        /// </summary>
        /// <param name="t"></param>
        public object Insert(BaseEntity t, ISession altSession = null)
        {
            ISession session = altSession ?? this.Session;

            object o = 0;

            if (session.Transaction != null && session.Transaction.IsActive)
            {
                //if (HttpContext.Current.Session != null)
                    // seteamos en el contexto de la DB con un SP el usuario que hace la operacion - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    //UserDBContextSPFactory.SetUserToDBContext(session, HttpContext.Current.Session["usuario"].ToString());

                o = session.Save(t);
                // hacemos flush para que la entidad tenga seteado el usuario - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                session.Flush();

                //if (HttpContext.Current.Session != null)
                    // Limpiamos con un SP el usuario seteado en el contexto de la BD - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    //UserDBContextSPFactory.CleanUserToDBContext(session);
            }
            else
            {
                using (var trx = session.BeginTransaction())
                {
                    o = session.Save(t);
                    trx.Commit();
                }
            }

            return o;
        }

        /// <summary>
        /// Inserta un objeto, tipado en T, en la BBDD
        /// </summary>
        /// <param name="t"></param>
        public object Insert<T>(T t, ISession altSession = null) where T : class
        {
            ISession session = altSession ?? this.Session;
            object o = 0;

            if (session.Transaction != null && session.Transaction.IsActive)
            {
                if (HttpContext.Current.Session != null)
                    // seteamos en el contexto de la DB con un SP el usuario que hace la operacion - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    //UserDBContextSPFactory.SetUserToDBContext(session, HttpContext.Current.Session["usuario"].ToString());

                o = session.Save(t);
                // hacemos flush para que la entidad tenga seteado el usuario - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                session.Flush();

                //if (HttpContext.Current.Session != null)
                    // Limpiamos con un SP el usuario seteado en el contexto de la BD - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    //UserDBContextSPFactory.CleanUserToDBContext(session);
            }
            else
            {
                using (var trx = session.BeginTransaction())
                {
                    o = session.Save(t);
                    trx.Commit();
                }
            }

            return o;
        }

        /// <summary>
        /// Metodo utilizado para hacer rollback del usuario seteado en el contexto de la base de datos.
        /// </summary>
        public void RollbackUserDBContext(ISession altSession = null)
        {
            //if(altSession != null)
            //    UserDBContextSPFactory.CleanUserToDBContext(altSession);
            //else
            //    UserDBContextSPFactory.CleanUserToDBContext(this.Session);
        }

        /// <summary>
        /// Hace un insert o update del objeto dependiendo si es o no transient.
        /// SIN FLUSH
        /// </summary>
        /// <param name="t"></param>
        public void SaveOrUpdate(BaseEntity t, ISession altSession = null)
        {
            ISession session = altSession ?? this.Session;

            if (session.Transaction != null && session.Transaction.IsActive)
            {
                //Este chequeo verifica que tenemos un contexto de sesion y no entramos a la api de seguridad
                if(HttpContext.Current.Session != null)
                // seteamos en el contexto de la DB con un SP el usuario que hace la operacion - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    UserDBContextSPFactory.SetUserToDBContext(session, HttpContext.Current.Session["usuario"].ToString());

                session.SaveOrUpdate(t);
                // hacemos flush para que la entidad tenga seteado el usuario - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                session.Flush();

                if (HttpContext.Current.Session != null)
                    // Limpiamos con un SP el usuario seteado en el contexto de la BD - SOLO NECESARIO PARA ENTIDADES SIGAF, PARA EL RESTO NO HACE NADA
                    UserDBContextSPFactory.CleanUserToDBContext(session);
            }
            else
            {
                using (var trx = session.BeginTransaction())
                {
                    session.SaveOrUpdate(t);
                    trx.Commit();
                }
            }
        }

        /// <summary>
        /// Hace un merge del objeto persistido con el objeto modificado.
        /// SIN FLUSH
        /// </summary>
        /// <param name="t"></param>
        public void Merge(BaseEntity t, ISession altSession = null)
        {
            if (altSession != null)
                altSession.Merge(t);
            else
                this.Session.Merge(t);
        }

        #endregion

        /// <summary>
        /// Genera un condicion de Disjuncion para una propiedad de una lista de objetos pasados por parametro.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="lista"></param>
        /// <returns></returns>
        protected Disjunction SetOrs(string property, IList<object> lista)
        {
            var disjunction = Expression.Disjunction();

            foreach (var item in lista)
            {
                disjunction.Add(Expression.Eq(property, item));
            }

            return disjunction;
        }

        /// <summary>
        /// Metodo para setear filtro de busqueda a un Criteria
        /// IMPORTANTE: debe ser implementado por las subclases.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="filter"></param>
        protected virtual void SetFilters(ICriteria criteria, System.Type filter)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region // Transaccion //
        /// <summary>
        /// Metodo para iniciar una transaccion.
        /// </summary>
        /// <returns></returns>
        public Transaction BeginTransaction(ISession altSession = null)
        {
            ISession session = altSession ?? this.Session;
            if (this.Transaction != null && this.Transaction.IsActive)
            {
                return this.Transaction;
            }
            else
            {
                this.Transaction = new Transaction();
                this.Transaction.BeginTransaction(session);
                return this.Transaction;
            }
        }

        /// <summary>
        /// Metodo para realizar el commit de una transaccion abierta.
        /// </summary>
        public void Commit()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
            }
        }

        /// <summary>
        /// Metodo para realizar el commit de una transaccion pasada por parametro.
        /// </summary>
        public void Commit(Transaction trx)
        {
            trx.Commit();
        }

        /// <summary>
        /// Metodo para deshacer operaciones crud hechas en una transaccion abierta.
        /// </summary>
        public void Rollback()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Rollback();
            }
        }

        /// <summary>
        /// Metodo para deshacer operaciones crud hechas en una transaccion pasada por parametro.
        /// </summary>
        public void Rollback(Transaction trx)
        {
            trx.Rollback();
        }
        #endregion

        #region // Metodos Privados //
        /// <summary>
        /// Metodo privado de busqueda, para un Criteria dado y un filtro
        /// </summary>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IList<BaseEntity> Search(ICriteria list, System.Type filter)
        {
            this.SetFilters(list, filter);
            return list.List<BaseEntity>();
        }

        /// <summary>
        /// Devuelve un Response con una lista de todos los objetos tipados en T de la base de datos según la consulta SQL recibida
        /// </summary>
        /// <returns></returns>
        public IList<BaseEntity> GetBySql(System.Type persistent, string sql, string parametro)
        {
            IList<BaseEntity> lista = new List<BaseEntity>();
            IQuery query = Session.CreateSQLQuery(sql).AddEntity(parametro, persistent);
            return query.List<BaseEntity>();
        }

        /// <summary>
        /// Devuelve una lista de todos los objetos tipados en T de la base de datos según la consulta SQL recibida
        /// </summary>
        /// <returns></returns>
        public IList<T> GetBySql<T>(string sql, string parametro) where T : class
        {
            IList<BaseEntity> lista = new List<BaseEntity>();
            IQuery query = Session.CreateSQLQuery(sql).AddEntity(parametro, typeof(T));
            return query.List<T>();
        }
        #endregion

        #region Otros
        /// <summary>
        /// El metodo devuelve el nombre de la base de datos actual.
        /// </summary>
        /// <returns></returns>
        public string ObtenerNombreBaseDeDatos()
        {

            ISession session = this.Session;
            try
            {
                IQuery query = session.CreateSQLQuery("SELECT GLOBAL_NAME FROM GLOBAL_NAME");
                var resultado = query.UniqueResult();

                return resultado.ToString();
            }
            catch (Exception e)
            {
                session.Dispose();
                throw e;
            }

        }
        #endregion
    }
}
