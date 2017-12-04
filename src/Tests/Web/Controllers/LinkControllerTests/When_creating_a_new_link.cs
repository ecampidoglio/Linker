using System;
using System.Web.Http.Results;
using FluentAssertions;
using Linker.Model;
using Linker.Web.Controllers;
using NSubstitute;
using Xunit;

namespace Linker.Tests.Web.Controllers.LinkControllerTests
{
    public class When_creating_a_new_link
    {
        [Fact]
        public void Should_return_an_http_created_result()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("example", "http://example.com");

            result.Should().BeOfType<CreatedAtRouteNegotiatedContentResult<string>>();
        }

        [Fact]
        public void Should_return_the_route_name_for_the_created_link()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("example", "http://example.com");

            result.Should().BeOfType<CreatedAtRouteNegotiatedContentResult<string>>()
                  .Which.RouteName.Should().Be("Follow");
        }

        [Fact]
        public void Should_return_the_id_of_the_created_link()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("example", "http://example.com");

            result.Should().BeOfType<CreatedAtRouteNegotiatedContentResult<string>>()
                  .Which.RouteValues.Should().ContainKey("id").And.ContainValue("example");
        }

        [Fact]
        public void Should_return_the_url_of_the_created_link()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("example", "http://example.com");

            result.Should().BeOfType<CreatedAtRouteNegotiatedContentResult<string>>()
                  .Which.Content.Should().Be("http://example.com");
        }

        [Fact]
        public void Should_save_a_link_with_the_specified_id_and_url()
        {
            var saveLink = Substitute.For<ISaveLinks>();
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                saveLink);

            sut.Create("example", "http://example.com");

            saveLink.Received().WithIdAndUrl("example", new Uri("http://example.com"));
        }
    }
}
