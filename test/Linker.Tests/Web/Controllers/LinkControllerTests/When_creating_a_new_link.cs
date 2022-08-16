using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using NSubstitute;

namespace Linker.Tests
{
    public class When_creating_a_new_link : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly FormUrlEncodedContent _httpContent;

        public When_creating_a_new_link(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            _httpContent = new FormUrlEncodedContent(
                new[] { new KeyValuePair<string, string>("url", "http://example.com") });
        }

        [Fact]
        public async Task Should_return_an_http_created_result()
        {
            var result = await _client.PutAsync("/link/id", _httpContent);

            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Should_return_the_redirect_location_for_the_created_link()
        {
            var result = await _client.PutAsync("/link/id", _httpContent);

            result.Should().BeOfType<HttpResponseMessage>()
                  .Which.Headers.Should().ContainSingle(kv => kv.Key == "Location" && kv.Value.First() == "http://localhost/id");
        }

        [Fact]
        public async Task Should_return_the_url_of_the_created_link()
        {
            var result = await _client.PutAsync("/link/id", _httpContent);

            var content = await result.Content.ReadAsStringAsync();

            content.Should().Be("\"http://example.com\"");
        }

        [Fact]
        public async Task Should_save_a_link_with_the_specified_id_and_url()
        {
            await _client.PutAsync("/link/id", _httpContent);

            _factory.SaveLinks.Received().WithIdAndUrl("id", new Uri("http://example.com"));
        }
    }
}
