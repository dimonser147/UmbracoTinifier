using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tinifier.Core.Models.API;
using System.Web.Script.Serialization;
using Tinifier.Core.Services.Interfaces;
using Tinifier.Core.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Tinifier.Core.Services.Services
{
    public class TinyPNGConnectorService : ITinyPNGConnector
    {
        private string _tinifyAddress;
        private JavaScriptSerializer _serializer;
        private ISettingsService _settingsService;

        public TinyPNGConnectorService()
        {
            _settingsService = new SettingsService();
            _tinifyAddress = PackageConstants.TinyPngUrl;
            _serializer = new JavaScriptSerializer();
        }

        public async Task<TinyResponse> TinifyByteArray(byte[] imageByteArray)
        {
            TinyResponse tinyResponse;
            ByteArrayContent byteContent = new ByteArrayContent(imageByteArray);

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

        public async Task<TinyResponse> TinifyJsonObject(string imageUrl)
        {
            TinyResponse tinyResponse;

            var source = new TinyJsonObject
            {
                Source = new Source
                {
                    Url = imageUrl
                }
            };

            var content = new StringContent(_serializer.Serialize(source).ToLower(), Encoding.UTF8, "application/json");

            try
            {
                var responseResult = await CreateRequest(content);
                tinyResponse = _serializer.Deserialize<TinyResponse>(responseResult);
                tinyResponse.Output.IsOptimized = true;
            }
            catch (HttpRequestException ex)
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
            string message;
            HttpResponseMessage response;
            var apiKey = _settingsService.GetSettings().ApiKey;
            var authKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + apiKey));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authKey);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.BaseAddress = new Uri(_tinifyAddress);

                try
                {
                    response = await client.PostAsync("/shrink", inputData as HttpContent);
                }
                catch (TaskCanceledException)
                {
                    throw new HttpRequestException(PackageConstants.TooBigImage);
                }
               
                if (!response.IsSuccessStatusCode)
                {
                    message = (int)response.StatusCode + response.ReasonPhrase;
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
            IEnumerable<string> headerValues = response.Headers.GetValues("Compression-Count");
            var compressionHeader = headerValues.FirstOrDefault();

            if(compressionHeader == null)
            {
                throw new HttpRequestException(HttpStatusCode.BadRequest + "Bad request");
            }

            var currentMonthRequests = int.Parse(compressionHeader);

            return currentMonthRequests;           
        }
    }
}
