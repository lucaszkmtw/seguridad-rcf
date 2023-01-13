using NHibernate;
using NHibernate.Persister.Entity;
using System;

namespace TGP.Seguridad.DataAccess.Infrastructure.Listeners
{
    public class BaseListener : EmptyInterceptor
    {
        protected static readonly string UpdtDate = "FechaActualizacion", UpdtUser = "UsuarioActualizacion", InsDate = "FechaAlta", InsUser = "UsuarioAlta", version = "Version";

        protected void SetState(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            this.SetState(persister.PropertyNames, state, propertyName, value);
        }

        protected void SetState(string[] propertyNames, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(propertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }

        protected void SetEntity(Trigger trigger, bool insert)
        {
            if (insert)
            {
                trigger.Audit.FechaAlta = trigger.Date;
                trigger.Audit.UsuarioAlta = trigger.User;
                trigger.Audit.Version = 0;
            }
            trigger.Audit.FechaActualizacion = trigger.Date;
            trigger.Audit.UsuarioActualizacion = trigger.User;
            trigger.Audit.Version++;
        }
    }
}
