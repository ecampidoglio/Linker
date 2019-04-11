using System;
using FluentAssertions;
using Linker.Model;
using Linker.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Linker.Tests
{
    public class When_creating_a_new_link_without_id
    {
        [Fact]
        public void Should_return_an_http_created_result()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("http://example.com");

            result.StatusCode().Should().Be(201);
        }

        [Fact]
        public void Should_return_the_route_name_for_the_created_link()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("http://example.com");

            result.Should().BeOfType<CreatedAtRouteResult>()
                .Which.RouteName.Should().Be("Follow");
        }

        [Fact]
        public void Should_return_the_generated_id_of_the_created_link()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("http://example.com");

            result.Should().BeOfType<CreatedAtRouteResult>()
                .Which.RouteValues.Should().ContainKey("id").And.NotBeNull();
        }

        [Fact]
        public void Should_return_the_url_of_the_created_link()
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create("http://example.com");

            result.Should().BeOfType<CreatedAtRouteResult>()
                .Which.Value.Should().Be("http://example.com");
        }

        [Fact]
        public void Should_save_a_link_with_specified_url_and_the_returned_id()
        {
            var saveLink = Substitute.For<ISaveLinks>();
            var sut = new LinkController(Substitute.For<IRetrieveLinks>(), saveLink);

            var id = sut.Create("http://example.com").CreatedLinkId();

            saveLink.Received().WithIdAndUrl(id, new Uri("http://example.com"));
        }

        [Fact]
        public void Should_save_links_for_the_same_url_with_different_ids()
        {
            var saveLink = Substitute.For<ISaveLinks>();
            var sut = new LinkController(Substitute.For<IRetrieveLinks>(), saveLink);

            var ids = new[]
            {
                sut.Create("http://example.com").CreatedLinkId(),
                sut.Create("http://example.com").CreatedLinkId()
            };

            ids.Should().OnlyHaveUniqueItems();
        }
    }
}
