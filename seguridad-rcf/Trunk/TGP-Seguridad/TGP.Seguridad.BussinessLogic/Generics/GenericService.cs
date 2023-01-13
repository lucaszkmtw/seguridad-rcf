using NHibernate;
using System;
using System.Collections.Generic;
using TGP.Seguridad.BussinessLogic.Dto;
using TGP.Seguridad.DataAccess;
using TGP.Seguridad.DataAccess.Generics;
using TGP.Seguridad.DataAccess.Infrastructure.Transactions;

namespace TGP.Seguridad.BussinessLogic.Generics
{

    /// <summary>
    /// Clase que modela los metodo basico de CRUD de la aplicacion
    /// En ella encontraremos los medios que permiten la tranformacion de la informacion
    /// enviada desde el cliente y las capas de entidades, y viceversa
    /// DTO => Entidad
    /// Entidad => DTO
    /// </summary>
    public class GenericService
    {
        #region // Variables //

        private GenericDataAccessService genericDataAccessService = GenericDataAccessService.GetInstance;


        private static GenericService instance;

        public static GenericService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GenericService();
                }
                return instance;
            }
        }

        #endregion

        protected GenericService() { }


        #region // Metodos Publicos //

        /// <summary>
        /// Metodo que abre una transaccion desde la bussiness
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        protected Transaction BeginTransaction(ISession session = null)
        {
            return genericDataAccessService.BeginTransaction(session);
        }

        /// <summary>
        /// Metodo que devuelve la session a la bussines
        /// </summary>
        /// <returns></returns>
        protected ISession Session()
        {
            return genericDataAccessService.GetSession();
        }

        /// <summary>
        /// Metodo que devuelve una session segun el archivo de configuracion que pasemos
        /// </summary>
        /// <param name="configPath">Path del archivo de config</param>
        /// <returns></returns>
        protected ISession GetSessionPorConfig(string configPath)
        {
            return genericDataAccessService.GetSessionPorConfig(configPath);
        }

        #region CONSULTA

        /// <summary>
        /// Metodo que devuelve el version de un elemento T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected int GetVersion<T>(long id, ISession session = null) where T : class
        {
            string[] propiedades = new string[] { "Version" };

            //Usar dynamic es una forma simple de "Castear" un objeto,
            //sabemos que T es de tipo BaseEntity siempre asi q en vez de castear
            //digo q es dinamico y le indico que me devuelva el Version, eso lo hace en tiempo de ejecucion
            dynamic entity = this.GetById<T>(id, propiedades, session);
            return entity.Version;
        }


        /// <summary>
        /// Devuelve un objeto Response con una lista de todos los objetos del tipo solicititado.
        /// </summary>
        /// <param name="baseDtoType">Tipo de objeto a consulta</param>
        /// <returns>Retorna un objeto Response que contiene la lista las metadata</returns>
        protected IList<BaseEntity> GetAll(System.Type entityType, ISession session = null)
        {
            //Se consulta y se carga la respuesta, la cual es retornada en el metodo
            return genericDataAccessService.GetAll(entityType, session);
        }

        /// <summary>
        /// Devuelve un objeto Response con una lista de todos los objetos del tipo solicititado.
        /// </summary>
        /// <param name="baseDtoType">Tipo de objeto a consulta</param>
        /// <returns>Retorna un objeto Response que contiene la lista las metadata</returns>
        protected IList<BaseEntity> GetAll(System.Type entityType, string[] propiedades, ISession session = null)
        {
            //Se consulta y se carga la respuesta, la cual es retornada en el metodo
            return genericDataAccessService.GetAll(entityType, propiedades, session);
        }

        /// <summary>
        /// Devuelve una lista de todos los objetos del tipo solicititado.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected IList<T> GetAll<T>(ISession session = null) where T : class
        {
            return genericDataAccessService.GetAll<T>(session);
        }

        /// <summary>
        /// Devuelve una lista de todos los objetos del tipo solicititado.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected IList<T> GetAll<T>(string[] propiedades, ISession session = null) where T : class
        {
            return genericDataAccessService.GetAll<T>(propiedades, session);
        }


        /// <summary>
        /// Devuelve un objeto Response con una lista de todos los objetos del tipo solicitado.
        /// </summary>
        /// <param name="entityType">Tipo de objeto a consulta</param>
        /// <returns>Retorna la lista las metadata</returns>
        protected IList<BaseEntity> GetBySql(System.Type entityType, string sql, string parametro)
        {
            //Se consulta y se carga la respuesta, la cual es retornada en el metodo
            return genericDataAccessService.GetBySql(entityType, sql, parametro);
        }

        /// <summary>
        /// Devuelve un objeto Response con una lista de todos los objetos del tipo solicitado.
        /// </summary>
        /// <param name="entityType">Tipo de objeto a consulta</param>
        /// <returns>Retorna la lista las metadata</returns>
        protected IList<T> GetBySql<T>(string sql, string parametro) where T : class
        {
            //Se consulta y se carga la respuesta, la cual es retornada en el metodo
            return genericDataAccessService.GetBySql<T>(sql, parametro);
        }

        /// <summary>
        /// Devuelve un int con un count de todos los objetos del tipo solicitado con el search especificado.
        /// </summary>
        /// <param name="baseDtoType">Tipo de objeto a consulta</param>
        /// <returns>Retorna un objeto Response que contiene la lista las metadata</returns>
        protected int GetCountByCriteria(Search search, Type entityType, ISession session = null)
        {
            //Se consulta y se carga la respuesta, la cual es retornada en el metodo
            return genericDataAccessService.GetCountByCriteria(search, entityType, session);
        }

        /// <summary>
        /// Devuelve un int con un count de todos los objetos del tipo solicitado con el search especificado.
        /// </summary>
        /// <param name="baseDtoType">Tipo de objeto a consulta</param>
        /// <returns>Retorna un objeto Response que contiene la lista las metadata</returns>
        protected int GetCountByCriteria<T>(Search search, ISession session = null) where T : class
        {
            //Se consulta y se carga la respuesta, la cual es retornada en el metodo
            return genericDataAccessService.GetCountByCriteria<T>(search, session);
        }

        /// <summary>
        /// Devuelve todos los objetos del tipo solicitado con el search especificado.
        /// </summary>
        /// <param name="baseDtoType">Tipo de objeto a consulta</param>
        /// <returns>Retorna un objeto Response que contiene la lista las metadata</returns>
        protected IList<T> GetByCriteria<T>(Search search, ISession session = null, bool distinct = false) where T : class
        {
            //Se consulta y se carga la respuesta, la cual es retornada en el metodo
            return genericDataAccessService.GetByCriteria<T>(search, distinct, session);
        }

        /// <summary>
        /// Devuelve de todos los objetos del tipo solicitado con el search especificado.
        /// </summary>
        /// <param name="baseDtoType">Tipo de objeto a consulta</param>
        /// <returns>Retorna un objeto Response que contiene la lista las metadata</returns>
        protected IList<T> GetByCriteria<T>(Search search, string[] propiedades, ISession session = null, bool distinct = false) where T : class
        {
            //Se consulta y se carga la respuesta, la cual es retornada en el metodo
            return genericDataAccessService.GetByCriteria<T>(search, propiedades, distinct, session);
        }

        /// <summary>
        /// Retorna un objeto DTO cuyo id viene por parametro junto con el typo dto solicitado, que mediante este 
        /// ultimo dato se determina la entidad a consultar.
        /// </summary>
        /// <param name="id">clave de identificacion de la entidad a recuperar</param>
        /// <param name="baseDtoType">tipo de entidad Dto</param>
        /// <returns></returns>
        protected BaseEntity GetById(long id, Type entityType, ISession session = null)
        {
            return genericDataAccessService.GetById(id, entityType, session);
        }

        /// <summary>
        /// Retorna un objeto DTO cuyo id viene por parametro junto con el typo dto solicitado, que mediante este 
        /// ultimo dato se determina la entidad a consultar.
        /// </summary>
        /// <param name="id">clave de identificacion de la entidad a recuperar</param>
        /// <param name="baseDtoType">tipo de entidad Dto</param>
        /// <returns></returns>
        protected BaseEntity GetById(long id, Type entityType, string[] propiedades, ISession session = null)
        {
            return genericDataAccessService.GetById(id, entityType, propiedades, session);
        }

        /// <summary>
        /// Retorna un objeto T cuyo id viene por parametro junto con el typo dto solicitado, que mediante este 
        /// ultimo dato se determina la entidad a consultar.
        /// </summary>
        /// <param name="id">clave de identificacion de la entidad a recuperar</param>
        /// <param name="baseDtoType">tipo de entidad Dto</param>
        /// <returns></returns>
        protected T GetById<T>(long id, ISession session = null) where T : class
        {
            return genericDataAccessService.GetById<T>(id, session);
        }
        /// <summary>
        /// Retorna un objeto T cuyo id viene por parametro junto con el typo dto solicitado, que mediante este 
        /// ultimo dato se determina la entidad a consultar.
        /// </summary>
        /// <param name="id">clave de identificacion de la entidad a recuperar</param>
        /// <param name="baseDtoType">tipo de entidad Dto</param>
        /// <returns></returns>
        protected T GetById<T>(long id, string[] propiedades, ISession session = null) where T : class
        {
            return genericDataAccessService.GetById<T>(id, propiedades, session);
        }


        /// <summary>
        /// Metodo para ir a buscar un objeto a la base por id, sin importar si es Id o compisiteId.
        /// </summary>
        /// <param name="ids">diccionario con el conjuntos de ids y sus valores para ir a buscar un objeto a la base</param>
        /// <param name="baseDtoType">tipo de la entidad dto a buscar</param>
        /// <returns>la entidad encontrada</returns>
        protected BaseEntity GetById(Dictionary<string, object> ids, Type entityType, ISession session = null)
        {
            return genericDataAccessService.GetById(ids, entityType, session);
        }

        /// <summary>
        /// Metodo para ir a buscar un objeto a la base por id, sin importar si es Id o compisiteId.
        /// </summary>
        /// <param name="ids">diccionario con el conjuntos de ids y sus valores para ir a buscar un objeto a la base</param>
        /// <param name="baseDtoType">tipo de la entidad dto a buscar</param>
        /// <returns>la entidad encontrada</returns>
        protected BaseEntity GetById(Dictionary<string, object> ids, Type entityType, string[] propiedades, ISession session = null)
        {
            return genericDataAccessService.GetById(ids, entityType, propiedades, session);
        }

        /// <summary>
        /// Metodo para ir a buscar un objeto a la base por id, sin importar si es Id o compisiteId.
        /// </summary>
        /// <param name="ids">diccionario con el conjuntos de ids y sus valores para ir a buscar un objeto a la base</param>
        /// <param name="baseDtoType">tipo de la entidad dto a buscar</param>
        /// <returns>la entidad encontrada</returns>
        protected T GetById<T>(Dictionary<string, object> ids, ISession session = null) where T : class
        {
            return genericDataAccessService.GetById<T>(ids, session);
        }

        /// <summary>
        /// Metodo para ir a buscar un objeto a la base por id, sin importar si es Id o compisiteId.
        /// </summary>
        /// <param name="ids">diccionario con el conjuntos de ids y sus valores para ir a buscar un objeto a la base</param>
        /// <param name="baseDtoType">tipo de la entidad dto a buscar</param>
        /// <returns>la entidad encontrada</returns>
        protected T GetById<T>(Dictionary<string, object> ids, string[] propiedades, ISession session = null) where T : class
        {
            return genericDataAccessService.GetById<T>(ids, propiedades, session);
        }
        #endregion


        #region CRUD

        #region SaveOrUpdate
        /// <summary>
        /// Inserta un objeto tipado en D, utilizando la capa de servicio, y retorna su ID.
        /// </summary>
        /// <param name="baseDto">Objeto DTO recibido desde la capa del cliente</param>
        protected void SaveOrUpdate(BaseEntity entity, ISession session = null)
        {
            //Entidad de destino, que se setea con el resultado de Mapear
            try
            {
                genericDataAccessService.SaveOrUpdate(entity, session);
            }
            catch (Exception e)
            {
                throw e;
            }

        } 
        #endregion

        #region Insert
        /// <summary>
        /// Inserta un objeto tipado en D, utilizando la capa de servicio, y retorna su ID.
        /// </summary>
        /// <param name="baseDto">Objeto DTO recibido desde la capa del cliente</param>
        protected object Insert(BaseEntity entity, ISession session = null)
        {
            //Entidad de destino, que se setea con el resultado de Mapear
            try
            {
                return genericDataAccessService.Insert(entity, session);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        /// <summary>
        /// Inserta un objeto tipado en D, utilizando la capa de servicio, y retorna su ID.
        /// </summary>
        /// <param name="baseDto">Objeto DTO recibido desde la capa del cliente</param>
        protected object InsertAll(IList<BaseEntity> entity, bool commit = true, ISession session = null)
        {
            //Entidad de destino, que se setea con el resultado de Mapear
            try
            {
                return genericDataAccessService.InsertAll(entity, commit, session);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Inserta un objeto tipado en D, utilizando la capa de servicio, y retorna su ID.
        /// </summary>
        /// <param name="baseDto">Objeto DTO recibido desde la capa del cliente</param>
        protected object InsertAll<T>(IList<T> entity, bool commit = true, ISession session = null) where T : class
        {
            //Entidad de destino, que se setea con el resultado de Mapear
            try
            {
                return genericDataAccessService.InsertAll<T>(entity, commit, session);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        #region Update
        /// <summary>
        /// Actualiza un objeto tipado en D, utilizando la capa de servicio.
        /// </summary>
        /// <param name="baseDto">Objeto DTO recibido desde la capa del cliente</param>
        protected void Update(BaseEntity entity, ISession session = null)
        {
            genericDataAccessService.Update(entity, session);
        }

        /// <summary>
        /// Actualiza un objeto tipado en D, utilizando la capa de servicio.
        /// </summary>
        /// <param name="baseDto">Objeto DTO recibido desde la capa del cliente</param>
        protected void UpdateAll(IList<BaseEntity> entity, bool commit = true, ISession session = null)
        {
            genericDataAccessService.UpdateAll(entity, commit, session);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Elimina un objeto cuyo ID es pasado por parametro, utilizando la capa de servicio.
        /// </summary>
        /// <param name="id">clave de identificacion de la entidad a borrar</param>
        /// <param name="baseDtoType">tipo de entidad Dto</param>
        protected void Delete(Dictionary<string, object> ids, Type entityType, ISession session = null)
        {
            genericDataAccessService.Delete(ids, entityType, session);
        }

        /// <summary>
        /// Elimina un objeto cuyo ID es pasado por parametro, utilizando la capa de servicio.
        /// </summary>
        /// <param name="id">clave de identificacion de la entidad a borrar</param>
        /// <param name="baseDtoType">tipo de entidad Dto</param>
        protected void Delete<T>(Dictionary<string, object> ids, ISession session = null) where T : class
        {
            genericDataAccessService.Delete<T>(ids, session);
        }

        /// <summary>
        /// Elimina un objeto cuyo ID es pasado por parametro, utilizando la capa de servicio.
        /// </summary>
        /// <param name="id">clave de identificacion de la entidad a borrar</param>
        /// <param name="baseDtoType">tipo de entidad Dto</param>
        protected void Delete(long id, Type entityType, ISession session = null)
        {
            genericDataAccessService.Delete(id, entityType, session);
        }

        /// <summary>
        /// Elimina un objeto cuyo ID es pasado por parametro, utilizando la capa de servicio.
        /// </summary>
        /// <param name="id">clave de identificacion de la entidad a borrar</param>
        /// <param name="baseDtoType">tipo de entidad Dto</param>
        protected void Delete<T>(long id, ISession session = null) where T : class
        {
            genericDataAccessService.Delete<T>(id, session);
        }

        protected void DeleteAll<T>(IList<long> ids, bool commit = true, ISession session = null) where T : class
        {
            genericDataAccessService.DeleteAll<T>(ids, commit, session);
        }

        protected void DeleteAll(IList<long> ids, Type entityType, bool commit = true, ISession session = null)
        {
            genericDataAccessService.DeleteAll(ids, entityType, commit, session);
        }

        protected void DeleteAll<T>(List<Dictionary<string, object>> ids, bool commit = true, ISession session = null) where T : class
        {
            genericDataAccessService.DeleteAll<T>(ids, commit, session);
        }

        protected void DeleteAll(List<Dictionary<string, object>> ids, Type entityType, bool commit = true, ISession session = null)
        {
            genericDataAccessService.DeleteAll(ids, entityType, commit, session);
        }
        #endregion

        #endregion

        #region OTROS
        
        public string ObtenerNombreBaseDeDatos()
        {
            return genericDataAccessService.ObtenerNombreBaseDeDatos();
        }
        #endregion
        #endregion
    }
}
