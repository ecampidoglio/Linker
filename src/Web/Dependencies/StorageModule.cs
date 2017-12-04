using Autofac;
using Linker.Model;
using Module = Autofac.Module;

namespace Linker.Web.Dependencies
{
    public class StorageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterInMemoryLinkStore(builder);
        }

        private static void RegisterInMemoryLinkStore(ContainerBuilder builder)
        {
            builder
                .RegisterType<StoreLinksInMemory>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
