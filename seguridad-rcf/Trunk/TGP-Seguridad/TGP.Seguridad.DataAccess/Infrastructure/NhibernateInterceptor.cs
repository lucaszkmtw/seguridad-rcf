using NHibernate;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using Oracle.DataAccess.Client;
using TGP.Seguridad.DataAccess.Infrastructure.Listeners;

namespace TGP.Seguridad.DataAccess.Infrastructure
{
    public class NHibernateInterceptor : BaseListener
    {
        private string sClientId;
        //private Trigger trigger;

        public NHibernateInterceptor()
        {
            this.sClientId = string.Empty;
        }

        public NHibernateInterceptor(string clientId)
        {
            sClientId = clientId;
        }

        private ISession _session;
        public override void SetSession(ISession session)
        {
            this._session = session;
            ((OracleConnection)session.Connection).ClientId = sClientId;
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            
            return base.OnSave(entity, id, state, propertyNames, types);
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            ISessionImplementor sessimpl = _session.GetSessionImplementation();
            IEntityPersister persister = sessimpl.GetEntityPersister(entity.ToString(), id);
            //EntityMode mode = _session.GetSessionImplementation().EntityMode;

            if (persister.IsVersioned)
            {
                object version = persister.GetVersion(entity);
                object currentVersion = persister.GetCurrentVersion(id, sessimpl);

                if (!persister.VersionType.IsEqual(currentVersion, version))
                    throw new StaleObjectStateException(persister.EntityName, id);
            }

            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

    }
}
