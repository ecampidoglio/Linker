using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Linker.Tests
{
    public class When_creating_a_new_link_without_id : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly FormUrlEncodedContent _httpContent;

        public When_creating_a_new_link_without_id(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            _httpContent = new FormUrlEncodedContent(
                new[] { new KeyValuePair<string, string>("url", "http://example.com") });
        }

        [Fact]
        public async Task Should_return_an_http_created_result()
        {
            var result = await _client.PostAsync("/link", _httpContent);

            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Should_return_the_redirect_location_for_the_created_link()
        {
            var result = await _client.PostAsync("/link", _httpContent);

            result.Should().BeOfType<HttpResponseMessage>()
                .Which.Headers.Should().ContainSingle(kv => kv.Key == "Location");
        }

        [Fact]
        public async Task Should_return_the_url_of_the_created_link()
        {
            var result = await _client.PostAsync("/link", _httpContent);

            var content = await result.Content.ReadAsStringAsync();

            content.Should().Be("\"http://example.com\"");
        }

        [Fact]
        public async Task Should_save_a_link_with_specified_url_and_the_returned_id()
        {
            var result = await _client.PostAsync("/link", _httpContent);

            var id = GetLinkId(result.Headers);

            _factory.SaveLinks.Received().WithIdAndUrl(id, new Uri("http://example.com"));
        }

        [Fact]
        public async Task Should_save_links_for_the_same_url_with_different_ids()
        {
            var ids = new List<string>();

            var result = await _client.PostAsync("/link", _httpContent);

            ids.Add(GetLinkId(result.Headers));

            result = await _client.PostAsync("/link", _httpContent);

            ids.Add(GetLinkId(result.Headers));

            ids.Should().OnlyHaveUniqueItems();
        }

        private static string GetLinkId(HttpResponseHeaders headers)
        {
            var url = headers.First(h => h.Key == "Location").Value.First();
            return new Uri(url).AbsolutePath.TrimStart('/');
        }
    }
}
