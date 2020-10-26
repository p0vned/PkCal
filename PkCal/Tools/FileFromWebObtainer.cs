using PkCal.Models;
using RestSharp;
using System;
using System.Net;

namespace PkCal.Tools
{
    class FileFromWebObtainer
    {
        private RestClient _client;
        private Uri _uri;
        public string Content { get; private set; }

        public FileFromWebObtainer(Uri calendarEndpoint)
        {
            _uri = calendarEndpoint;
            _client = new RestClient(_uri);
        }

        public Result GetCalendarData()
        {
            var acceptedContentType = "text/calendar";

            _client.Timeout = 5000;

            var request = new RestRequest(Method.GET);
            var response = _client.Execute(request);
            
            var statusResponse = response.StatusCode;

            if (statusResponse.Equals(HttpStatusCode.OK) && response.ContentType.Contains(acceptedContentType))
            {
                Content = response.Content;
                return new SuccessResult();
            }

            return new FailedResult(response.ErrorMessage);
        }
    }
}
