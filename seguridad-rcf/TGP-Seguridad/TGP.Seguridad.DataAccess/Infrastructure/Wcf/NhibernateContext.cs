using System.ServiceModel;

namespace TGP.Seguridad.DataAccess.Infrastructure.Wcf
{
    public class NhibernateContext
    {
        public static NhibernateContextExtension Current()
        {
            return OperationContext.Current.
                InstanceContext.
                Extensions.
                Find<NhibernateContextExtension>();
        }
    }
}