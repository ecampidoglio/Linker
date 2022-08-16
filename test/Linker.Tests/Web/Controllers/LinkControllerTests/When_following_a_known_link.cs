using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Linker.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using Xunit;

namespace Linker.Tests
{
    public class When_following_a_known_link : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public When_following_a_known_link(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions {AllowAutoRedirect = false});
        }

        [Fact]
        public async Task Should_return_an_http_moved_permanently_result()
        {
            _factory.GetLinks.WithId("id").Returns(new Link("id,", "http://example.com"));

            var result = await _client.GetAsync("/id");

            result.StatusCode.Should().Be(HttpStatusCode.MovedPermanently);
        }

        [Fact]
        public async Task Should_return_the_url_of_the_link()
        {
            _factory.GetLinks.WithId("id").Returns(new Link("id,", "http://example.com"));

            var result = await _client.GetAsync("/id");

            result.Headers.Should().ContainSingle(kv => kv.Key == "Location" && kv.Value.First() == "http://example.com/");
        }
    }
}
