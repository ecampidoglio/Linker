using Linker.Web.Configuration;
using Owin;

namespace Linker.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.ConfigureWebApi();
        }
    }
}
