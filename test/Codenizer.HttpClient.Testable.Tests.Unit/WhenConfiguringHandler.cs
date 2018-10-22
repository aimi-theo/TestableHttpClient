﻿using System;
using System.Net.Http;
using Xunit;
using FluentAssertions;

namespace Codenizer.HttpClient.Testable.Tests.Unit
{
    public class WhenConfiguringHandler
    {
        [Fact]
        public void GivenExceptionShouldBeThrown_ExceptionIsThrown()
        {
            var handler = new MessageHandler();
            var client = new System.Net.Http.HttpClient(handler);

            handler.ShouldThrow(new Exception("BANG"));

            Action action = () => client.GetAsync("https://tempuri.org/api/hello");

            action
                .Should()
                .Throw<Exception>()
                .WithMessage("BANG");
        }

        [Fact]
        public void GivenTwoResponsesForSamePath_MultipleResponsesExceptionIsThrown()
        {
            var handler = new MessageHandler();
            var client = new System.Net.Http.HttpClient(handler);

            handler.RespondTo("/api/hello");
            handler.RespondTo("/api/hello");

            Action action = () => client.GetAsync("https://tempuri.org/api/hello");

            action.Should().Throw<MultipleResponsesConfiguredException>();
        }

        [Fact]
        public void GivenTwoResponsesForSamePathAndQueryString_MultipleResponsesExceptionIsThrown()
        {
            var handler = new MessageHandler();
            var client = new System.Net.Http.HttpClient(handler);

            handler.RespondTo("/api/hello?foo=bar");
            handler.RespondTo("/api/hello?foo=bar");

            Action action = () => client.GetAsync("https://tempuri.org/api/hello?foo=bar");

            action.Should().Throw<MultipleResponsesConfiguredException>();
        }

        [Fact]
        public void GivenTwoResponsesForSamePathButDifferentQueryString_NoMultipleResponsesExceptionIsThrown()
        {
            var handler = new MessageHandler();
            var client = new System.Net.Http.HttpClient(handler);

            handler.RespondTo("/api/hello?foo=bar");
            handler.RespondTo("/api/hello?foo=qux");

            Action action = () => client.GetAsync("https://tempuri.org/api/hello?foo=bar");

            action.Should().NotThrow<MultipleResponsesConfiguredException>();
        }

        [Fact]
        public void GivenTwoResponsesForSamePathAndQueryStringAndMethod_MultipleResponsesExceptionIsThrown()
        {
            var handler = new MessageHandler();
            var client = new System.Net.Http.HttpClient(handler);

            handler.RespondTo(HttpMethod.Get, "/api/hello?foo=bar");
            handler.RespondTo(HttpMethod.Get, "/api/hello?foo=bar");

            Action action = () => client.GetAsync("https://tempuri.org/api/hello?foo=bar");

            action.Should().Throw<MultipleResponsesConfiguredException>();
        }

        [Fact]
        public void GivenTwoResponsesForSamePathAndQueryStringButDifferentMethod_NoMultipleResponsesExceptionIsThrown()
        {
            var handler = new MessageHandler();
            var client = new System.Net.Http.HttpClient(handler);

            handler.RespondTo(HttpMethod.Get, "/api/hello?foo=bar");
            handler.RespondTo(HttpMethod.Post, "/api/hello?foo=bar");

            Action action = () => client.GetAsync("https://tempuri.org/api/hello?foo=bar");

            action.Should().NotThrow<MultipleResponsesConfiguredException>();
        }
    }
}
