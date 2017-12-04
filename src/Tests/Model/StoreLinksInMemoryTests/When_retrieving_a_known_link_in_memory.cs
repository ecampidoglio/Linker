using FluentAssertions;
using Linker.Model;
using Xunit;

namespace Linker.Tests.Model.StoreLinksInMemoryTests
{
    public class When_retrieving_a_known_link_in_memory
    {
        [Fact]
        public void Should_return_the_link_with_the_specified_id()
        {
            var sut = new StoreLinksInMemory();

            var result = sut.WithId("known-link-id");

            result?.Id.Should().Be("known-link-id");
        }

        [Fact]
        public void Should_return_the_href_of_the_specified_link()
        {
            var sut = new StoreLinksInMemory();

            var result = sut.WithId("known-link-id");

            result?.Href.Should().Be("http://example.com");
        }
    }
}
