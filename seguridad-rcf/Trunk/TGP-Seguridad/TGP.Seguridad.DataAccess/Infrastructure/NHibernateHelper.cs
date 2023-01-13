using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Event;
using Oracle.DataAccess.Client;
using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using TGP.Seguridad.DataAccess.Infrastructure.Listeners;
using TGP.Seguridad.DataAccess.Infrastructure.Wcf;
using Configuration = NHibernate.Cfg.Configuration;

namespace TGP.Seguridad.DataAccess.Infrastructure
{

    /// <summary>
    /// Enumeracion con el tipo de Contexto, por lo general se utiliza el WEB=1
    /// </summary>
    public enum Context
    {
        Web = 1,
        Wcf = 2
    }

    /// <summary>
    /// Clase de soporte para NHibernate
    /// </summary>
    public sealed class NHibernateHelper
    {
        #region // Variables Privadas //
        private const string CurrentSessionKey = "nhibernate.current_session";
        private static readonly ISessionFactory sessionFactory;
        private static readonly Configuration config;
        private static readonly Context _context;
        #endregion

        #region // Constructor //
        /// <summary>
        /// Construtor 
        /// </summary>
        static NHibernateHelper()
        {
            Assembly mappingsAssembly = Assembly.Load("TGP.Seguridad.DataAccess");
            config = GetConfiguration();
            sessionFactory = Fluently.Configure(config).Mappings(m =>
            {
                m.HbmMappings.AddFromAssembly(mappingsAssembly);
                m.FluentMappings.AddFromAssembly(mappingsAssembly);
            }).BuildSessionFactory();

            var ctx = ConfigurationManager.AppSettings["Context"];
            var result = Enum.TryParse<Context>(ctx, true, out _context);

            if (!result)
                throw new Exception("Nhibernate init error: el valor de Context en AppConfig no es correcto o no ha sido especificado. Valores posibles: Web, Wcf");
        }
        #endregion

        #region // Propiedad Publica //
        /// <summary>
        /// Contexto actual
        /// </summary>
        public static Context CurrentContext { get { return _context; } }
        #endregion

        #region // Metodos Publicos //
        /// <summary>
        /// Metodo que realiza la apertura de la sesion
        /// </summary>
        /// <returns></returns>
        public static ISession OpenSession()
        {
            return sessionFactory.OpenSession();
        }

        public static ISession GetSessionFromConfig(string configPath)
        {
            //string CurrentSessionKey = "nhibernate.new_connection";
            ISessionFactory sessionFactoryNewConnection;
            Configuration configNewConnection;
            Context _contextNewConnection;
            var cfg = new Configuration();

            cfg.Configure(HttpRuntime.AppDomainAppPath + configPath);

            cfg.SetListener(ListenerType.Delete, new DeleteEventListener());

            /**
             * ********* ENVERS *********************************
             * Se invoca el metodo con la configuracion de Envers.
             * Esto se debe hacer siempre antes de el BuildSessionFactory()
             */
            configNewConnection = AddConfigEnvers(cfg);
            /****************************************************/
            

            Assembly mappingsAssembly = Assembly.Load("TGP.Seguridad.DataAccess");
            sessionFactoryNewConnection = Fluently.Configure(configNewConnection).Mappings(m =>
            {
                m.HbmMappings.AddFromAssembly(mappingsAssembly);
                m.FluentMappings.AddFromAssembly(mappingsAssembly);
            }).BuildSessionFactory();

            var ctx = ConfigurationManager.AppSettings["Context"];
            var result = Enum.TryParse<Context>(ctx, true, out _contextNewConnection);
            return sessionFactoryNewConnection.OpenSession();
        }

        /// <summary>
        /// Metodo que obtiene la sesion actual de usuario
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static ISession GetCurrentSession(string username)
        {
            ISession currentSession = default(ISession);

            NHibernateInterceptor nHibernateInterceptor = new NHibernateInterceptor(username.ToUpper());

            switch (CurrentContext)
            {
                case Context.Web:
                    {
                        HttpContext context = HttpContext.Current;
                        currentSession = context.Items[CurrentSessionKey] as ISession;

                        if (currentSession == null)
                        {
                            currentSession = sessionFactory.OpenSession(nHibernateInterceptor);
                            context.Items[CurrentSessionKey] = currentSession;
                        }
                        else
                        {
                            ((OracleConnection)currentSession.Connection).ClientId = (username.ToUpper());
                        }
                    }
                    break;
                case Context.Wcf:
                    {
                        currentSession = NhibernateContext.Current() == null ? sessionFactory.OpenSession(nHibernateInterceptor) : NhibernateContext.Current().Session;
                    }
                    break;
                default:
                    break;
            }

            return currentSession;
        }

        /// <summary>
        /// Metodo que obtiene la sesion actual
        /// </summary>
        /// <returns></returns>
        public static ISession GetCurrentSession()
        {
            ISession currentSession = default(ISession);

            switch (CurrentContext)
            {
                case Context.Web:
                    {
                        HttpContext context = HttpContext.Current;
                        currentSession = context.Items[CurrentSessionKey] as ISession;

                        if (currentSession == null)
                        {
                            currentSession = sessionFactory.OpenSession(new EmptyInterceptor());
                            context.Items[CurrentSessionKey] = currentSession;
                        }
                    }
                    break;
                case Context.Wcf:
                    {
                        currentSession = NhibernateContext.Current() == null ? sessionFactory.OpenSession() : NhibernateContext.Current().Session;
                    }
                    break;
                default:
                    break;
            }

            return currentSession;
        }

        /// <summary>
        /// Metodo que cierra la sesion
        /// </summary>
        public static void CloseSession()
        {
            switch (CurrentContext)
            {
                case Context.Web:
                    {
                        HttpContext context = HttpContext.Current;
                        ISession currentSession = context.Items[CurrentSessionKey] as ISession;

                        if (currentSession == null)
                        {
                            // No current session
                            return;
                        }

                        currentSession.Close();
                        context.Items.Remove(CurrentSessionKey);
                    }
                    break;
                case Context.Wcf:
                    {
                        var operation = NhibernateContext.Current();

                        if (operation != null)
                            operation.Session.Dispose();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Metodo que cierra la sesion de una factoria
        /// </summary>
        public static void CloseSessionFactory()
        {
            if (sessionFactory != null)
            {
                sessionFactory.Close();
            }
        }

        /// <summary>
        /// Metodo que recupera la configuracion del Nhibernate
        /// </summary>
        /// <returns></returns>
        private static Configuration GetConfiguration()
        {
            var cfg = new Configuration();
            cfg.Configure();

            cfg.SetListener(ListenerType.Delete, new DeleteEventListener());

            /**
             * ********* ENVERS *********************************
             * Se invoca el metodo con la configuracion de Envers.
             * Esto se debe hacer siempre antes de el BuildSessionFactory()
             */
            Configuration HBcfg = AddConfigEnvers(cfg);
            /****************************************************/

            return HBcfg;
        }

        /// <summary>
        /// Configuracion con los parametros para Envers
        /// </summary>
        private static Configuration AddConfigEnvers(NHibernate.Cfg.Configuration cfg)
        {
            // Indicamos a envers el sufijo de las tablas de dominio que guardan la historia
            Configuration enversConf = NhConfigurationExtension.SetEnversProperty(cfg, ConfigurationKey.AuditTableSuffix, "_H");
            // Indicamos a envers el nombre del campo id de la tabla de Revision
            enversConf = NhConfigurationExtension.SetEnversProperty(enversConf, ConfigurationKey.RevisionFieldName, "ID_REVISION_INFO");
            // Indicamos a envers el nombre del campo que determina el tipo de revision (0 = Alta - 1 = Modificacion - 2 = Borrado)
            enversConf = NhConfigurationExtension.SetEnversProperty(enversConf, ConfigurationKey.RevisionTypeFieldName, "TE_REVISION_TYPE");
            // Indicamos a envers que las propiedades usadas para el loqueo optimista no sean auditadas.
            enversConf = NhConfigurationExtension.SetEnversProperty(enversConf, ConfigurationKey.DoNotAuditOptimisticLockingField, false);
            // Indicamos a envers que cuando se borre una tupla de una tabla auditada genere de igual manera la nueva tupla en la tabla _H
            enversConf = NhConfigurationExtension.SetEnversProperty(enversConf, ConfigurationKey.StoreDataAtDelete, true);
            // Le indicamos a envers que se integre con NHibernate
            enversConf = NhConfigurationExtension.IntegrateWithEnvers(enversConf);
            return enversConf;
        }

        #endregion
    }
}
