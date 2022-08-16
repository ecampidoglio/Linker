using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Linker.Tests
{
    public class When_getting_the_metadata_about_an_unknown_link : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public When_getting_the_metadata_about_an_unknown_link(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Should_return_an_http_not_found_result()
        {
            var result = await _client.GetAsync("/link/unknown-link-id");

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
