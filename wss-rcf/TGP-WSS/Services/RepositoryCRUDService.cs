using NHibernate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TGP.WSS.Services
{
    /*
     * Este servicio es un singleton ya que necesitamos que se maneje la session de NHibernate como una unica instancia.
     */
 
    public abstract class RepositoryCRUDService<T> where T: class
    {
        //ISession session = NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["ConexionDB"]);

        //public static ISession getSessionSEGURIDAD() { return NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["ConexionDBSEGURIDAD"]); }
        public static ISession getSession() { return NHibernateSessionManager.Instance.GetSessionFrom(ConfigurationManager.AppSettings["SegDB"]); }


        public void Save(object objeto)
        {
            getSession().Save(objeto);
            getSession().Flush();
        }

        public void SaveWithOutFlush(object objeto)
        {
            getSession().Save(objeto);
        }

        public void SaveOrUpdate(object objeto)
        {
            getSession().SaveOrUpdate(objeto);
            getSession().Flush();
        }

        public void Update(object objeto)
        {
            getSession().Update(objeto);
            getSession().Flush();
        }

        public void UpdateWithOutFlush(object objeto)
        {
            getSession().Update(objeto);
        }

        public void Delete(object objeto)
        {
            getSession().Delete(objeto);
            getSession().Flush();
        }

        public IList<T> GetAll()
        {
            return getSession().QueryOver<T>().List<T>().ToList();
        }

         public T Get(long id)
        {
            return getSession().Get<T>(id);
        }

         public void Clear()
         {
             getSession().Clear();
         }


    }
}