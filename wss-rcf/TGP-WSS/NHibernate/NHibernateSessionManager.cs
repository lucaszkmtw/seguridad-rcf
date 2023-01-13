using NHibernate;
using NHibernate.Cache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Caching;

namespace NHibernate
{
    public class NHibernateSessionManager
    {


        #region Thread-safe, lazy Singleton

        /// <summary>
        /// This is a thread-safe, lazy singleton.
        /// See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>

        public static NHibernateSessionManager Instance
        {
            get
            {
                return Nested.NHibernateSessionManager;
            }
        }

        /// <summary>
        /// Private constructor to enforce singleton
        /// </summary>

        private NHibernateSessionManager() { }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>

        private class Nested
        {
            static Nested() { }
            internal static readonly NHibernateSessionManager
               NHibernateSessionManager = new NHibernateSessionManager();
        }

        #endregion

        /// <summary>
        /// This method attempts to find a session factory
        /// in the <see cref="HttpRuntime.Cache" /> 
        /// via its config file path; if it can't be
        /// found it creates a new session factory and adds
        /// it the cache. Note that even though this uses HttpRuntime.Cache,
        /// it should still work in Windows applications; see
        /// http://www.codeproject.com/csharp/cacheinwinformapps.asp
        /// for an examination of this.
        /// </summary>
        /// <param name="sessionFactoryConfigPath">Path location
        /// of the factory config</param>

        private ISessionFactory GetSessionFactoryFor(string sessionFactoryConfigPath)
        {
            string pathFile = System.AppDomain.CurrentDomain.RelativeSearchPath;
            pathFile = pathFile.Substring(0, pathFile.Length - 3);
            sessionFactoryConfigPath = pathFile + sessionFactoryConfigPath;

            if (string.IsNullOrEmpty(sessionFactoryConfigPath))
                throw new ArgumentNullException("sessionFactoryConfigPath" +
                          " may not be null nor empty");

            //  Attempt to retrieve a cached SessionFactory from the HttpRuntime's cache.
            ISessionFactory sessionFactory =
              (ISessionFactory)HttpRuntime.Cache.Get(sessionFactoryConfigPath);

            //  Failed to find a cached SessionFactory so make a new one.
            if (sessionFactory == null)
            {
                if (!File.Exists(sessionFactoryConfigPath))
                    // It would be more appropriate to throw
                    // a more specific exception than ApplicationException

                    throw new ApplicationException(
                        "The config file at '" + sessionFactoryConfigPath +
                        "' could not be found");

                NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration();
                cfg.Configure(sessionFactoryConfigPath);

                //  Now that we have our Configuration object, create a new SessionFactory

                sessionFactory = cfg.BuildSessionFactory();

                if (sessionFactory == null)
                {
                    throw new InvalidOperationException(
                      "cfg.BuildSessionFactory() returned null.");
                }

                HttpRuntime.Cache.Add(sessionFactoryConfigPath,
                            sessionFactory, null, DateTime.Now.AddDays(7),
                    TimeSpan.Zero, CacheItemPriority.High, null);
            }

            return sessionFactory;
        }

        public void RegisterInterceptorOn(string sessionFactoryConfigPath,
                                          IInterceptor interceptor)
        {
            ISession session = (ISession)contextSessions[sessionFactoryConfigPath];

            if (session != null && session.IsOpen)
            {
                throw new CacheException("You cannot register " +
                      "an interceptor once a session has already been opened");
            }

            GetSessionFrom(sessionFactoryConfigPath, interceptor);
        }

        public ISession GetSessionFrom(string sessionFactoryConfigPath)
        {
            return GetSessionFrom(sessionFactoryConfigPath, null);
        }
        
        private ISession GetSessionFrom(string sessionFactoryConfigPath,
                                        IInterceptor interceptor)
        {

            ISession session = (ISession)contextSessions[sessionFactoryConfigPath];
            if (session == null)
            {
                if (interceptor != null)
                {
                    session = GetSessionFactoryFor(
                       sessionFactoryConfigPath).OpenSession(interceptor);
                }
                else
                {
                    session =
                     GetSessionFactoryFor(sessionFactoryConfigPath).OpenSession();
                }

                contextSessions[sessionFactoryConfigPath] = session;
            }

            if (session == null)
                // It would be more appropriate to throw
                // a more specific exception than ApplicationException

                throw new ApplicationException("session was null");

            return session;
        }

        public void CloseSessionOn(string sessionFactoryConfigPath)
        {
            ISession session = (ISession)contextSessions[sessionFactoryConfigPath];
            contextSessions.Remove(sessionFactoryConfigPath);

            if (session != null && session.IsOpen)
            {
                session.Close();
            }
        }

        public void BeginTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction =
              (ITransaction)contextTransactions[sessionFactoryConfigPath];

            if (transaction == null)
            {
                transaction = GetSessionFrom(sessionFactoryConfigPath).BeginTransaction();
                contextTransactions.Add(sessionFactoryConfigPath, transaction);
            }
        }

        public void CommitTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction =
              (ITransaction)contextTransactions[sessionFactoryConfigPath];

            try
            {
                if (transaction != null && !transaction.WasCommitted
                                        && !transaction.WasRolledBack)
                {
                    transaction.Commit();
                    contextTransactions.Remove(sessionFactoryConfigPath);
                }
            }
            catch (HibernateException)
            {
                RollbackTransactionOn(sessionFactoryConfigPath);
                throw;
            }
        }

        public void RollbackTransactionOn(string sessionFactoryConfigPath)
        {
            ITransaction transaction =
              (ITransaction)contextTransactions[sessionFactoryConfigPath];

            try
            {
                contextTransactions.Remove(sessionFactoryConfigPath);

                if (transaction != null && !transaction.WasCommitted
                                        && !transaction.WasRolledBack)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                CloseSessionOn(sessionFactoryConfigPath);
            }
        }

        /// <summary>
        /// Since multiple databases may be in use, there may be one transaction per database 
        /// persisted at any one time. The easiest way to store them is via a hashtable
        /// with the key being tied to session factory.
        /// </summary>

        private Hashtable contextTransactions
        {
            get
            {
                if (CallContext.GetData("CONTEXT_TRANSACTIONS") == null)
                {
                    CallContext.SetData("CONTEXT_TRANSACTIONS", new Hashtable());
                }

                return (Hashtable)CallContext.GetData("CONTEXT_TRANSACTIONS");
            }
        }

        /// <summary>
        /// Since multiple databases may be in use, there may be one session per database 
        /// persisted at any one time. The easiest way to store them is via a hashtable
        /// with the key being tied to session factory.
        /// </summary>

        private Hashtable contextSessions
        {
            get
            {
                if (CallContext.GetData("CONTEXT_SESSIONS") == null)
                {
                    CallContext.SetData("CONTEXT_SESSIONS", new Hashtable());
                }

                return (Hashtable)CallContext.GetData("CONTEXT_SESSIONS");
            }
        }
    
    
    }
}