using System;
using FluentAssertions;
using Linker.Model;
using Xunit;

namespace Linker.Tests.Model.StoreLinksInMemoryTests
{
    public class When_saving_a_new_link_in_memory
    {
        [Fact]
        public void Should_save_a_link_with_the_specified_id()
        {
            var sut = new StoreLinksInMemory();

            sut.WithIdAndUrl("id", new Uri("http://example.com"));

            sut.WithId("id").Should().NotBeNull();
        }

        [Fact]
        public void Should_save_a_link_with_the_specified_url()
        {
            var sut = new StoreLinksInMemory();
            var url = new Uri("http://example.com");

            sut.WithIdAndUrl("id", url);

            sut.WithId("id")?.Href.Should().Be(url);
        }
    }
}
