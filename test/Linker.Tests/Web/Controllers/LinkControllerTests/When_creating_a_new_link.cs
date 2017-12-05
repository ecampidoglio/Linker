using System;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using FluentAssertions;
using NSubstitute;
using Linker.Web.Controllers;
using Linker.Model;

namespace Linker.Tests
{
    public class When_creating_a_new_link
    {
        [Fact]
        public void Should_return_an_http_created_result()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("id", "http://example.com");

            result.StatusCode().Should().Be(201);
        }

        [Fact]
        public void Should_return_the_route_name_for_the_created_link()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("id", "http://example.com");

            result.Should().BeOfType<CreatedAtRouteResult>()
                  .Which.RouteName.Should().Be("Follow");
        }

        [Fact]
        public void Should_return_the_route_id_for_the_created_link()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("id", "http://example.com");

            result.Should().BeOfType<CreatedAtRouteResult>()
                  .Which.RouteValues.Should().ContainKey("id").And.NotBeNull();
        }

        [Fact]
        public void Should_return_the_url_of_the_created_link()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("id", "http://example.com");

            result.Should().BeOfType<CreatedAtRouteResult>()
                  .Which.Value.Should().Be("http://example.com");
        }

        [Fact]
        public void Should_save_a_link_with_the_specified_id_and_url()
        {
            var saveLink = Substitute.For<ISaveLinks>();
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                saveLink);

            var result = sut.Create("id", "http://example.com");

            saveLink.Received().WithIdAndUrl("id", new Uri("http://example.com"));
        }
    }
}
