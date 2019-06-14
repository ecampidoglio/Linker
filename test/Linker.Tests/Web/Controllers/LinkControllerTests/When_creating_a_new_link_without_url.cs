using FluentAssertions;
using Linker.Model;
using Linker.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Linker.Tests
{
    public class When_creating_a_new_link_without_url
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_return_an_http_bad_request_result(string url)
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create(url);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_return_an_http_bad_request_result_even_with_a_provided_id(string url)
        {
            var sut = new LinkController(
                Substitute.For<IRetrieveLinks>(),
                Substitute.For<ISaveLinks>());

            var result = sut.Create(id: "example", url);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
