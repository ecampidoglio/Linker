using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using Module = Autofac.Module;

namespace Linker.Web.Dependencies
{
    public class WebApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterControllers(builder);
        }

        private static void RegisterControllers(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
        }
    }
}
