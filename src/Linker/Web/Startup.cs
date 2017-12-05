using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Linker.Web.Configuration;

namespace Linker.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddLinkInMemoryStore();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory log)
        {
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            log.AddConsole();
        }
    }
}
