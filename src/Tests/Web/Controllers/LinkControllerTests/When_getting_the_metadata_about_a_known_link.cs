using System.Web.Http.Results;
using FluentAssertions;
using Linker.Model;
using Linker.Tests.Extensions;
using Linker.Web.Controllers;
using NSubstitute;
using Xunit;

namespace Linker.Tests.Web.Controllers.LinkControllerTests
{
    public class When_getting_the_metadata_about_a_known_link
    {
        [Fact]
        public void Should_return_an_http_ok_result()
        {
            var getLink = Substitute.For<IRetrieveLinks>();
            getLink.WithId("id").Returns(new Link("id,", "http://example.com"));
            var sut = new LinkController(getLink, Substitute.For<ISaveLinks>());

            var result = sut.Metadata("id");

            result.Should().BeOfType<OkNegotiatedContentResult<Link>>();
        }

        [Fact]
        public void Should_return_a_link_with_the_specified_id()
        {
            var getLink = Substitute.For<IRetrieveLinks>();
            getLink.WithId("id").Returns(new Link("id", "http://example.com"));
            var sut = new LinkController(getLink, Substitute.For<ISaveLinks>());

            var result = sut.Metadata("id");

            result.ContentAs<Link>().Id.Should().Be("id");
        }

        [Fact]
        public void Should_return_the_href_of_the_link()
        {
            var getLink = Substitute.For<IRetrieveLinks>();
            getLink.WithId("id").Returns(new Link("id,", "http://example.com"));
            var sut = new LinkController(getLink, Substitute.For<ISaveLinks>());

            var result = sut.Metadata("id");

            result.ContentAs<Link>().Href.Should().Be("http://example.com");
        }
    }
}
