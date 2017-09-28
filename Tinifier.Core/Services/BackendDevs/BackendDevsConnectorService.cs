using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;

namespace Tinifier.Core.Services.BackendDevs
{
    public class BackendDevsConnectorService : IBackendDevsConnector
    {
        private readonly JavaScriptSerializer _serializer;

        public BackendDevsConnectorService()
        {
            _serializer = new JavaScriptSerializer();
        }

        public async Task SendStatistic(string domainName)
        {
            try
            {
                await CreateRequest(domainName);
            }
            catch(NotSuccessfullRequestException)
            {
                throw;
            }
        }

        private async Task CreateRequest(string domainName)
        {
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(PackageConstants.BackEndDevsUrl);
                client.DefaultRequestHeaders.TryAddWithoutValidation(PackageConstants.ContentTypeHeader, PackageConstants.ContentType);

                var content = new StringContent(_serializer.Serialize(domainName), Encoding.UTF8, PackageConstants.ContentType);

                try
                {
                    response = await client.PostAsync(PackageConstants.BackEndDevsPostStatistic, content);
                }
                catch (Exception ex)
                {
                    throw new NotSuccessfullRequestException(ex.Message);
                }

                if (!response.IsSuccessStatusCode)
                {
                    var message = (int)response.StatusCode + response.ReasonPhrase;
                    throw new NotSuccessfullRequestException(message);
                }
            }
        }
    }
}
