using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Tinifier.Core.Services.BackendDevs
{
    public class BackendDevsConnectorService : IBackendDevsConnector
    {
        public async void SendStatistic(string domainName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:6740");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("domainName", domainName)
                });

                var response = await client.PostAsync("/api/Membership/exists", content);

                if (!response.IsSuccessStatusCode)
                {
                    var message = (int)response.StatusCode + response.ReasonPhrase;
                    throw new HttpRequestException(message);
                }
            }
        }
    }
}
