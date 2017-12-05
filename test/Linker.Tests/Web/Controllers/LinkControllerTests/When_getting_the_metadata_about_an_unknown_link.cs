using Microsoft.AspNetCore.Mvc;
using Xunit;
using FluentAssertions;
using NSubstitute;
using Linker.Web.Controllers;
using Linker.Model;

namespace Linker.Tests
{
    public class When_getting_the_metadata_about_an_unknown_link
    {
        [Fact]
        public void Should_return_an_http_not_found_result()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Metadata("unknown-link-id");

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
