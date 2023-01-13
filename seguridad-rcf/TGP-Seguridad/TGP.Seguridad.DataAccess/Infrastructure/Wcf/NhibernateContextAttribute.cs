using NHibernate;
using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace TGP.Seguridad.DataAccess.Infrastructure.Wcf
{
    public class NhibernateContextAttribute : Attribute, IContractBehavior
    {
        public ISessionFactory SessionFactory { private get; set; }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint) { }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceContextInitializers.Add(new NhibernateContextInitializer());
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
    }
}