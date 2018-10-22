using System;
using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Xunit;

namespace Codenizer.HttpClient.Testable.Tests.Unit
{
    public class WhenHandlingRequest
    {
        [Fact]
        public async void GivenConfiguredResponseCodeOk_ResponseStatusIsOk()
        {
            var handler = new MessageHandler();
            var client = new System.Net.Http.HttpClient(handler);

            handler
                .RespondTo("/api/hello?foo=bar")
                .With(HttpStatusCode.OK);

            var response = await client.GetAsync("https://tempuri.org/api/hello?foo=bar");

            response
                .StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async void GivenConfiguredResponseBody_ResponseContainsBody()
        {
            var handler = new MessageHandler();
            var client = new System.Net.Http.HttpClient(handler);

            handler
                .RespondTo("/api/hello?foo=bar")
                .With(HttpStatusCode.OK)
                .AndContent("text/plain", "data");

            var response = await client.GetAsync("https://tempuri.org/api/hello?foo=bar");

            response
                .Content
                .ReadAsStringAsync()
                .Result
                .Should()
                .Be("data");
        }
        
        [Fact]
        public async void GivenResponseMediaTypeIsApplicationJson_ContentTypeIsSetWithCharsetOption()
        {
            var handler = new MessageHandler();
            var client = new System.Net.Http.HttpClient(handler);

            handler
                .RespondTo("/api/hello?foo=bar")
                .With(HttpStatusCode.OK)
                .AndContent("application/json", "data");

            var response = await client.GetAsync("https://tempuri.org/api/hello?foo=bar");

            response
                .Content
                .Headers
                .ContentType
                .Should()
                .Be(new MediaTypeHeaderValue("application/json") {CharSet = "utf-8"});
        }
    }
}