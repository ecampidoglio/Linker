using System.Web.Http.Results;
using FluentAssertions;
using Linker.Model;
using Linker.Web.Controllers;
using NSubstitute;
using Xunit;

namespace Linker.Tests.Web.Controllers.LinkControllerTests
{
    public class When_following_a_known_link
    {
        [Fact]
        public void Should_return_an_http_moved_result()
        {
            var getLink = Substitute.For<IRetrieveLinks>();
            getLink.WithId("id").Returns(new Link("id,", "http://example.com"));
            var sut = new LinkController(getLink, Substitute.For<ISaveLinks>());

            var result = sut.Follow("id");

            result.Should().BeOfType<RedirectResult>();
        }

        [Fact]
        public void Should_return_the_url_of_the_link()
        {
            var getLink = Substitute.For<IRetrieveLinks>();
            getLink.WithId("id").Returns(new Link("id,", "http://example.com"));
            var sut = new LinkController(getLink, Substitute.For<ISaveLinks>());

            var result = sut.Follow("id");

            result.Should().BeOfType<RedirectResult>()
                .Which.Location.Should().Be("http://example.com/");
        }
    }
}
