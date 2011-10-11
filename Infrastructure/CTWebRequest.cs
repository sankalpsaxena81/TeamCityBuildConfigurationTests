using System;
using System.Net;

namespace Infrastructure
{
   public class CTWebRequest : WebClient
        {
            private readonly int timeout;
            public CTWebRequest(int timeout)
            {
                this.timeout = timeout;
                Encoding = System.Text.Encoding.UTF8;
            }
            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                var httpRequest = request as HttpWebRequest;

                if (null != httpRequest)
                {
                    SetUpUserCredentials(httpRequest);
                    httpRequest.Timeout = timeout;
                    httpRequest.KeepAlive = true;
                }
                return request;
            }

            private void SetUpUserCredentials(HttpWebRequest httpRequest)
            {
                    httpRequest.Credentials = new NetworkCredential("ssaxena", "Welcome05", "sjrb");
            }
            public void AddHeader(string headerName, string headerValue)
            {
                Headers[headerName] = headerValue;
            }
        }
    }
