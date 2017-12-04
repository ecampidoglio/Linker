using FluentAssertions;
using Linker.Model;
using Xunit;

namespace Linker.Tests.Model.StoreLinksInMemoryTests
{
    public class When_retrieving_an_unknown_link_in_memory
    {
        [Fact]
        public void Should_return_null()
        {
            var sut = new StoreLinksInMemory();

            var result = sut.WithId("unknown-link-id");

            result.Should().BeNull();
        }
    }
}
