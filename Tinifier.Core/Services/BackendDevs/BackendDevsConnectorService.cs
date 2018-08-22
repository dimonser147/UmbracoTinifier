using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.API;

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
                var domainModel = new DomainModel { DomainName = domainName };

                try
                {
                    response = await client.PostAsJsonAsync(PackageConstants.BackEndDevsPostStatistic, domainModel);
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
