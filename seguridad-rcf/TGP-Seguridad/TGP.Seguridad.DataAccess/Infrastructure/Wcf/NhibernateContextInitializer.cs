using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using TGP.Seguridad.DataAccess.Infrastructure;

namespace TGP.Seguridad.DataAccess.Infrastructure.Wcf
{
    public class NhibernateContextInitializer : IInstanceContextInitializer
    {
        public void Initialize(InstanceContext instanceContext, Message message)
        {
            instanceContext.Extensions.Add(new NhibernateContextExtension(NHibernateHelper.OpenSession()));
        }
    }
}