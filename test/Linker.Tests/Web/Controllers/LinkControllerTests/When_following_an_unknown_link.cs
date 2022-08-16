using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Linker.Tests
{
    public class When_following_an_unknown_link : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public When_following_an_unknown_link(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Should_return_an_http_not_found_result()
        {
            var result = await _client.GetAsync("/unknown-link-id");

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
