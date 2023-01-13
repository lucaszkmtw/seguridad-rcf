using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TGP.Seguridad.Startup))]

namespace TGP.Seguridad
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                app.MapSignalR();
            }
            catch 
            {

            }
        }
    }
}