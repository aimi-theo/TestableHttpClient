﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Codenizer.HttpClient.Testable
{
    public class MessageHandler : HttpMessageHandler
    {
        private readonly List<RequestBuilder> _configuredRequests;
        private Exception _exceptionToThrow;

        public MessageHandler()
        {
            _configuredRequests = new List<RequestBuilder>();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(_exceptionToThrow != null)
            {
                throw _exceptionToThrow;
            }

            var matches = _configuredRequests.Where(r => r.PathAndQuery == request.RequestUri.PathAndQuery && r.Method == request.Method);

            if(!matches.Any())
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"No response configured for {request.RequestUri.PathAndQuery}")
                });
            }
            
            if(matches.Count() > 1)
            {
                throw new MultipleResponsesConfiguredException(matches.Count(), request.RequestUri.PathAndQuery);
            }

            var responseBuilder = matches.Single();

            var response = new HttpResponseMessage
            {
                StatusCode = responseBuilder.StatusCode
            };

            if (responseBuilder.Data != null)
            {
                response.Content = new StringContent(responseBuilder.Data, Encoding.UTF8, responseBuilder.MediaType);
            }

            return Task.FromResult(response);
        }

        public RequestBuilder RespondTo(string pathAndQuery)
        {
            return RespondTo(HttpMethod.Get, pathAndQuery);
        }

        public RequestBuilder RespondTo(HttpMethod method, string pathAndQuery)
        {
            var requestBuilder = new RequestBuilder(method, pathAndQuery);
            
            _configuredRequests.Add(requestBuilder);
            
            return requestBuilder;
        }

        public void ShouldThrow(Exception exception)
        {
            _exceptionToThrow = exception;
        }
    }
}
