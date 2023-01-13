using System.Web;

namespace TGP.Seguridad.DataAccess.Infrastructure
{
    internal class SessionWrapper
    {
        public static string GetUser()
        {
            if (NHibernateHelper.CurrentContext == Context.Web && HttpContext.Current != null)
                return HttpContext.Current.User.Identity.Name;
            else
                return string.Empty;
        }
    }
}
