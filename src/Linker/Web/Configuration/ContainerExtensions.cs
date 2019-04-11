using Microsoft.Extensions.DependencyInjection;
using Linker.Model;

namespace Linker.Web.Configuration
{
    internal static class ContainerExtensions
    {
        internal static IServiceCollection AddLinkInMemoryStore(
             this IServiceCollection container)
        {
            var memoryStore = new StoreLinksInMemory();

            return container
                .AddSingleton<ISaveLinks>(memoryStore)
                .AddSingleton<IRetrieveLinks>(memoryStore);
        }
    }
}
