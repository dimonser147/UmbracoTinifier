using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Tinifier.Core.Interfaces;
using Tinifier.Core.Models.API;
using System.Web.Script.Serialization;
using System.Net;

namespace Tinifier.Core.Services
{
    public class TCreateRequestService : ITCreateRequest
    {
        private string _apiKey;
        private string _authKey;
        private string _tinifyAddress;

        public TCreateRequestService()
        {
            _apiKey = "FA8HJIylmFJp7SkONjsHrchDlq7NmTKx";
            _authKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + _apiKey));
            _tinifyAddress = "https://api.tinify.com";
        }

        public async Task<string> CreateRequestByteArray(byte[] imageByteArray)
        {
            ByteArrayContent byteContent = new ByteArrayContent(imageByteArray);

            var responseResult = await CreateRequest(byteContent);
            return responseResult;
        }

        public async Task<string> CreateRequestJsonObject(string imageUrl)
        {
            var serializer = new JavaScriptSerializer();

            var source = new TinyJsonObject
            {
                Source = new Source
                {
                    Url = imageUrl
                }
            };

            var content = new StringContent(serializer.Serialize(source).ToLower(), Encoding.UTF8, "application/json");
            var responseResult = await CreateRequest(content);
            return responseResult;
        }

        public byte[] GetTinyImage(string url)
        {
            byte[] tinyImageBytes;

            using (var webClient = new WebClient())
            {
                tinyImageBytes = webClient.DownloadData(url);
            }

            return tinyImageBytes;
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
            }

            var responseResult = await response.Content.ReadAsStringAsync();
            return responseResult;
        }
    }
}
