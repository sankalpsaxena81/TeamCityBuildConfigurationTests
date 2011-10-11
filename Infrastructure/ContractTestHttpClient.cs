using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Infrastructure
{
    public class ContractTestHttpClient
    {
        private static readonly int DefaultClientTimeout = 60000;
        private const string DefaultContentType = "application/xml";

        private readonly int timeOutMillionSeconds;
        private readonly Dictionary<string, string> headers;

        public ContractTestHttpClient()
            : this(DefaultClientTimeout, DefaultContentType)
        {
        }

        public ContractTestHttpClient(int timeOutMillionSeconds)
            : this(timeOutMillionSeconds, DefaultContentType)
        {
        }

        public ContractTestHttpClient(string contentType)
            : this(DefaultClientTimeout, contentType)
        {
        }
        public ContractTestHttpClient(int timeOutMillionSeconds, string contentType)
        {
            this.timeOutMillionSeconds = timeOutMillionSeconds;
            headers = new Dictionary<string, string>();
            headers["Content-Type"] = contentType;
        }

        public void SetHeader(string key, string value)
        {
            if (!headers.ContainsKey(key))
                headers[key] = value;
        }

        public string GetHeader(string key)
        {
            return headers[key];
        }

        public ContractTestHttpClientResponse Get(string uri)
        {
            var httpClientResponse = InitializeClientResponse();
            return ExecuteRequest(
                httpClientResponse,
                uri,
                ContractTestHttpRequestType.Get,
                x => { httpClientResponse.Response = x.DownloadString(new Uri(uri)); });
        }

        public ContractTestHttpClientResponse Post(string uri, string content)
        {
            return Upload(uri, ContractTestHttpRequestType.Post, content);
        }

        public ContractTestHttpClientResponse Put(string uri, string content)
        {
            return Upload(uri, ContractTestHttpRequestType.Put, content);
        }

        public ContractTestHttpClientResponse Delete(string uri)
        {
            return Upload(uri, ContractTestHttpRequestType.Delete, string.Empty);
        }

        private ContractTestHttpClientResponse Upload(string uri, string httpMethod, string content)
        {
            var httpClientResponse = InitializeClientResponse();
            return ExecuteRequest(
                httpClientResponse,
                uri,
                httpMethod,
                x => { httpClientResponse.Response = x.UploadString(new Uri(uri), httpMethod, content); });
        }

        private ContractTestHttpClientResponse InitializeClientResponse()
        {
            return new ContractTestHttpClientResponse { Response = string.Empty, StatusCode = HttpStatusCode.OK };
        }

        private ContractTestHttpClientResponse ExecuteRequest(
            ContractTestHttpClientResponse response,
            string uri,
            string httpMethod,
            Action<CTWebRequest> action)
        {
            using (var webRequest = new CTWebRequest(timeOutMillionSeconds))
            {
                try
                {
                    AddHeaders(webRequest);
                    action(webRequest);
                }
                catch (WebException ex)
                {
                    ProcessWebException(ex, uri, response);
                }
                catch (Exception exception)
                {
                    ProcessUnknownException(exception, response, uri);
                }
                TransferHeaders(webRequest, response);
                return response;
            }
        }

        private void TransferHeaders(CTWebRequest request, ContractTestHttpClientResponse response)
        {
            if (request.ResponseHeaders == null) return;

            response.Headers = new Dictionary<string, string>(request.ResponseHeaders.Count);
            foreach (var key in request.ResponseHeaders.AllKeys)
            {
                response.Headers.Add(key, request.ResponseHeaders[key]);
            }
        }

        private void AddHeaders(CTWebRequest webRequest)
        {
            AddShawHeaders(webRequest);
            foreach (var key in headers.Keys)
            {
                if (headers[key] != null) webRequest.AddHeader(key, headers[key]);
            }
        }

        private void AddShawHeaders(CTWebRequest webRequest)
        {
//            webRequest.AddHeader(ShawHttpHeaders.TransactionId, Guid.NewGuid().ToString());
//            webRequest.AddHeader(ShawHttpHeaders.OriginatingUserId, HostEnvironmentInformation.UserName);
//            webRequest.AddHeader(ShawHttpHeaders.OriginatingIpAddress, GetIpAddress());
//            webRequest.AddHeader(ShawHttpHeaders.OriginatingHostName, Dns.GetHostName());
            webRequest.AddHeader(ShawHttpHeaders.OriginalModuleId, "Team City Build Configuration Tests");
        }

        private string GetIpAddress()
        {
            var ipAddressCollection = Dns.GetHostAddresses(Dns.GetHostName()).ToList();
            var ipAddress = ipAddressCollection.Find(address1 => AddressFamily.InterNetwork.Equals(address1.AddressFamily));
            return ipAddress == null ? string.Empty : ipAddress.ToString();
        }

        private void ProcessWebException(WebException exception, string uri, ContractTestHttpClientResponse httpClientResponse)
        {
            httpClientResponse.StatusCode = HttpStatusCode.InternalServerError;

            var webResponse = exception.Response;
            if (webResponse == null) return;

            var responseStream = webResponse.GetResponseStream();
            if (null != responseStream)
            {
                using (var reader = new StreamReader(responseStream))
                {
                    try
                    {
                        httpClientResponse.Response = reader.ReadToEnd();
                    }
                    catch (Exception e)
                    {
                        //TODO : Need to figure out the real cause
                    }
                }
            }

            var httpWebResponse = webResponse as HttpWebResponse;
            if (httpWebResponse != null)
                httpClientResponse.StatusCode = httpWebResponse.StatusCode;
        }

        private void ProcessUnknownException(Exception exception, ContractTestHttpClientResponse httpClientResponse, string uri)
        {
            httpClientResponse.StatusCode = HttpStatusCode.BadRequest;
            httpClientResponse.Response = exception.StackTrace;
        }
    }
    
}