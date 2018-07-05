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
using Tinifier.Core.Services.Settings;
using System.IO;
using Tinifier.Core.Models.Db;
using Umbraco.Core.IO;

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

        public async Task<TinyResponse> TinifyAsync(TImage tImage, IFileSystem fs)
        {
            byte[] imageBytes;
            TinyResponse tinyResponse;

            try
            {
                string path = fs.GetRelativePath(tImage.AbsoluteUrl);
                using (Stream file = fs.OpenFile(path))
                {
                    imageBytes = ReadFully(file);
                }
                tinyResponse = await TinifyByteArrayAsync(imageBytes).ConfigureAwait(false);
            }
            catch (Exception)
            {
                tinyResponse = new TinyResponse
                {
                    Output = new TinyOutput
                    {
                        Error = PackageConstants.ImageDeleted,
                        IsOptimized = false
                    }
                };
            }

            return tinyResponse;
        }


      

        private byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private async Task<TinyResponse> TinifyByteArrayAsync(byte[] imageByteArray)
        {
            //removed image limitation Feature #3215 UPD: just remove any limitation on an image size
            TinyResponse tinyResponse;
            //if(imageByteArray.Length > PackageConstants.MaxImageSize)
            //{
            //    tinyResponse = new TinyResponse
            //    {
            //        Output = new TinyOutput
            //        {
            //            Error = PackageConstants.TooBigImage,
            //            IsOptimized = false
            //        }
            //    };
            //}
            //else
            //{
                var byteContent = new ByteArrayContent(imageByteArray);
                try
                {
                    var responseResult = await CreateRequestAsync(byteContent).ConfigureAwait(false);
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
            //}           

            return tinyResponse;
        }

        private async Task<string> CreateRequestAsync<T>(T inputData)
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
                    response = await client.PostAsync(PackageConstants.TinyPngUri, inputData as HttpContent).ConfigureAwait(false);
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

            var responseResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return responseResult;
        }

      
        private int GetHeaderValue(HttpResponseMessage response)
        {
            var headerValues = response.Headers.GetValues(PackageConstants.TinyPngHeader);
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
