using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
