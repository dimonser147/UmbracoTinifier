using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tinifier.Core.Interfaces;
using Tinifier.Core.Models.API;
using System.Web.Script.Serialization;

namespace Tinifier.Core.Services
{
    public class TinyPNGConnectorService : ITinyPNGConnector
    {
        private string _apiKey;
        private string _authKey;
        private string _tinifyAddress;
        private JavaScriptSerializer _serializer;

        public TinyPNGConnectorService()
        {
            _apiKey = "FA8HJIylmFJp7SkONjsHrchDlq7NmTKx";
            _authKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + _apiKey));
            _tinifyAddress = "https://api.tinify.com";
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
            HttpResponseMessage response;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _authKey);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.BaseAddress = new Uri(_tinifyAddress);

                response = await client.PostAsync("/shrink", inputData as HttpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var message = (int)response.StatusCode + response.ReasonPhrase;
                    throw new HttpRequestException(message);
                }
            }

            var responseResult = await response.Content.ReadAsStringAsync();
            return responseResult;
        }
    }
}
