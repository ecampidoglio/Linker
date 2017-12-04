using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Linker.Web.Dependencies;

namespace Linker.Web.Configuration
{
    internal static class DependencyConfiguration
    {
        internal static void ConfigureDependencyResolution(
            this HttpConfiguration config)
        {
            config.DependencyResolver =
                new AutofacWebApiDependencyResolver(BuildContainer());
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<StorageModule>();
            builder.RegisterModule<WebApiModule>();

            return builder.Build();
        }
    }
}
