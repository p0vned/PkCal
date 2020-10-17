using RestSharp;
using System;
using System.Net;

namespace PkCal.Tools
{
    public class FileFromWebObtainer
    {
        private RestClient _client;
        private Uri _uri;
        public string Content { get; private set; }

        public FileFromWebObtainer(Uri calendarEndpoint)
        {
            _uri = calendarEndpoint;
            _client = new RestClient(_uri);
        }

        public bool GetCalendarData()
        {
            var acceptedContentType = "text/calendar";

            var request = new RestRequest(Method.GET);
            var response = _client.Execute(request);
            
            var statusResponse = response.StatusCode;

            if (statusResponse.Equals(HttpStatusCode.OK) && response.ContentType.Contains(acceptedContentType))
            {
                Content = response.Content;
                return true;
            }

            return false;
        }
    }
}
