using System;
using System.IO;
using System.Net;

namespace Core
{
    public class Request
    {
        private string baseUrl="https://teamcity.oss.prd";

        public string MakeGetRequest(string link)
        {
            Uri uri;
            if (!link.StartsWith(baseUrl))
            {
                uri = new Uri(baseUrl + link);
            }
            else
            {
                uri= new Uri(link);
            }
            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.Credentials = new NetworkCredential
            {
                UserName = "ssaxena",
                Password = "Welcome05",
            };
            webRequest.Method = "GET";
            WebResponse webResponse = webRequest.GetResponse();
            var dataStream = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }
    }
}