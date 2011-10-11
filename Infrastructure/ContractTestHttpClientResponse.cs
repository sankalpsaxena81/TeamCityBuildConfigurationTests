using System.Collections.Generic;
using System.Net;

namespace Infrastructure
{
    public class ContractTestHttpClientResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public string Response { get; set; }
    }
}