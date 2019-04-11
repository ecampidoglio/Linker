using System;
using Xunit;
using FluentAssertions;
using Linker.Model;

namespace Linker.Tests
{
    public class When_saving_an_existing_link_in_memory
    {
        [Fact]
        public void Should_replace_the_url_for_the_link_with_specified_id()
        {
            var sut = new StoreLinksInMemory();
            sut.WithIdAndUrl("id", new Uri("http://existing.com"));
            var newUrl = new Uri("http://example.com");

            sut.WithIdAndUrl("id", newUrl);

            sut.WithId("id")?.Href.Should().Be(newUrl);
        }
    }
}
