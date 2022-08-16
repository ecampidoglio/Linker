using System.Linq;
using Linker.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Linker.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public readonly ISaveLinks SaveLinks = Substitute.For<ISaveLinks>();
    public readonly IRetrieveLinks GetLinks = Substitute.For<IRetrieveLinks>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var saveLinks = services.Single(d => d.ServiceType == typeof(ISaveLinks));
            var retrieveLinks = services.Single(d => d.ServiceType == typeof(IRetrieveLinks));

            services.Remove(saveLinks);
            services.Remove(retrieveLinks);

            services.AddScoped(_ => SaveLinks);
            services.AddScoped(_ => GetLinks);
        });
    }
}
