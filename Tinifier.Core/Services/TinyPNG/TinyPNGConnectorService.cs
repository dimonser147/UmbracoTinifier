using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tinifier.Core.Models.API;
using System.Web.Script.Serialization;
using Tinifier.Core.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using Tinifier.Core.Services.Settings;

namespace Tinifier.Core.Services.TinyPNG
{
    public class TinyPNGConnectorService : ITinyPNGConnector
    {
        private readonly string _tinifyAddress;
        private readonly JavaScriptSerializer _serializer;
        private readonly ISettingsService _settingsService;

        public TinyPNGConnectorService()
        {
            _settingsService = new SettingsService();
            _tinifyAddress = PackageConstants.TinyPngUrl;
            _serializer = new JavaScriptSerializer();
        }

        public async Task<TinyResponse> SendImageToTinyPngService(string imageUrl)
        {
            var imageBytes = System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath($"~{imageUrl}"));
            var tinyResponse = await TinifyByteArray(imageBytes);

            return tinyResponse;
        }

        private async Task<TinyResponse> TinifyByteArray(byte[] imageByteArray)
        {
            TinyResponse tinyResponse;
            var byteContent = new ByteArrayContent(imageByteArray);

            try
            {
                var responseResult = await CreateRequest(byteContent);
                tinyResponse = _serializer.Deserialize<TinyResponse>(responseResult);
                tinyResponse.Output.IsOptimized = true;
            }
            catch(HttpRequestException ex)
            {
                tinyResponse = new TinyResponse
                {
                    Output = new TinyOutput
                    {
                        Error = ex.Message,
                        IsOptimized = false
                    }
                };
            }

            return tinyResponse;
        }

        private async Task<string> CreateRequest<T>(T inputData)
        {
            HttpResponseMessage response;
            var apiKey = _settingsService.GetSettings().ApiKey;
            var authKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(PackageConstants.ApiKey + apiKey));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(PackageConstants.BasicAuth, authKey);
                client.DefaultRequestHeaders.TryAddWithoutValidation(PackageConstants.ContentTypeHeader, PackageConstants.ContentType);
                client.BaseAddress = new Uri(_tinifyAddress);

                try
                {
                    response = await client.PostAsync(PackageConstants.TinyPngUri, inputData as HttpContent);
                }
                catch (TaskCanceledException)
                {
                    throw new HttpRequestException(PackageConstants.TooBigImage);
                }
               
                if (!response.IsSuccessStatusCode)
                {
                    var message = (int)response.StatusCode + response.ReasonPhrase;
                    throw new HttpRequestException(message);
                }
            }

            var currentMonthRequests = GetHeaderValue(response);
            _settingsService.UpdateSettings(currentMonthRequests);

            var responseResult = await response.Content.ReadAsStringAsync();
            return responseResult;
        }

        private int GetHeaderValue(HttpResponseMessage response)
        {
            var headerValues = response.Headers.GetValues(PackageConstants.TinyPNGHeader);
            var compressionHeader = headerValues.FirstOrDefault();

            if(compressionHeader == null)
            {
                throw new HttpRequestException(HttpStatusCode.BadRequest + PackageConstants.BadRequest);
            }

            var currentMonthRequests = int.Parse(compressionHeader);

            return currentMonthRequests;           
        }
    }
}
