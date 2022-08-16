using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Linker.Tests
{
    public class When_creating_a_new_link_with_an_invalid_url : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public When_creating_a_new_link_with_an_invalid_url(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("example.com")]
        [InlineData("https://")]
        [InlineData("not an URL")]
        public async Task Should_return_an_http_bad_request_result(string url)
        {
            var httpContent = new FormUrlEncodedContent(
                new[] { new KeyValuePair<string, string>("url", url) });

            var result = await _client.PutAsync("/link/id", httpContent);

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("example.com")]
        [InlineData("https://")]
        [InlineData("not an URL")]
        public async Task Should_return_an_http_bad_request_result_even_with_a_provided_id(string url)
        {
            var httpContent = new FormUrlEncodedContent(
                new[] { new KeyValuePair<string, string>("url", url) });

            var result = await _client.PutAsync("/link/example", httpContent);

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
