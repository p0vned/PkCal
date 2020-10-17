using RestSharp;
using System;

namespace PkCal.Tools
{
    public class FileFromWebObtainer
    {
        private RestClient _client;
        private Uri _uri;

        public FileFromWebObtainer(Uri calendarEndpoint)
        {
            _uri = calendarEndpoint;
            _client = new RestClient(_uri);
        }

        public void GetCalendarData()
        {
            var request = new RestRequest(Method.GET);
            var response = _client.Execute(request);
            var content = response.Content;
        }
    }
}
