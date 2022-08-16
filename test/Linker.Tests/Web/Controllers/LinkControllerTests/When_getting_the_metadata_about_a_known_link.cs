using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using NSubstitute;
using Linker.Model;

namespace Linker.Tests
{
    public class When_getting_the_metadata_about_a_known_link : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

        public When_getting_the_metadata_about_a_known_link(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Should_return_an_http_ok_result()
        {
            _factory.GetLinks.WithId("id").Returns(new Link("id,", "http://example.com"));

            var result = await _client.GetAsync("/link/id");

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Should_return_a_link_with_the_specified_id()
        {
            _factory.GetLinks.WithId("id").Returns(new Link("id", "http://example.com"));

            var result = await _client.GetAsync("/link/id");
            var content = await result.Content.ReadAsStringAsync();

            var link = JsonSerializer.Deserialize<Link>(content, _jsonSerializerOptions);
            link.Id.ToString().Should().Be("id");
        }

        [Fact]
        public async Task Should_return_the_href_of_the_link()
        {
            _factory.GetLinks.WithId("id").Returns(new Link("id", "http://example.com"));

            var result = await _client.GetAsync("/link/id");
            var content = await result.Content.ReadAsStringAsync();

            var link = JsonSerializer.Deserialize<Link>(content, _jsonSerializerOptions);

            link.Href.Should().Be("http://example.com");
        }
    }
}

