using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Linker.Tests
{
    public class When_creating_a_new_link_without_url : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public When_creating_a_new_link_without_url(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Should_return_an_http_bad_request_result(string url)
        {
            var httpContent = new FormUrlEncodedContent(
                new[] { new KeyValuePair<string, string>("url", url) });

            var result = await _client.PostAsync("/link", httpContent);

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Should_return_an_http_bad_request_result_even_with_a_provided_id(string url)
        {
            var httpContent = new FormUrlEncodedContent(
                new[] { new KeyValuePair<string, string>("url", url) });

            var result = await _client.PutAsync("/link/example", httpContent);

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
