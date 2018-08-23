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
using Tinifier.Core.Models.Db;
using Umbraco.Core.IO;
using Tinifier.Core.Repository.FileSystemProvider;
using Tinifier.Core.Services.History;

namespace Tinifier.Core.Services.TinyPNG
{
    public class TinyPNGConnectorService : ITinyPNGConnector
    {
        private readonly string _tinifyAddress;
        private readonly JavaScriptSerializer _serializer;
        private readonly ISettingsService _settingsService;
        private readonly IFileSystemProviderRepository _fileSystemProviderRepository;
        private readonly IImageHistoryService _imageHistoryService;

        public TinyPNGConnectorService()
        {
            _settingsService = new SettingsService();
            _tinifyAddress = PackageConstants.TinyPngUrl;
            _serializer = new JavaScriptSerializer();
            _fileSystemProviderRepository = new TFileSystemProviderRepository();
            _imageHistoryService = new TImageHistoryService();
        }

        public async Task<TinyResponse> TinifyAsync(TImage tImage, IFileSystem fs)
        {
            TinyResponse tinyResponse;
            var path = tImage.AbsoluteUrl;
            int.TryParse(tImage.Id, out var id);
            var settings = _settingsService.GetSettings();

            try
            {
                var imageBytes = GetImageBytesFromPath(tImage, fs, path);
                tinyResponse = await TinifyByteArrayAsync(imageBytes).ConfigureAwait(false);

                if(id > 0 && settings.EnableUndoOptimization)
                    _imageHistoryService.Create(tImage, imageBytes);
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

        private byte[] GetImageBytesFromPath(TImage tImage, IFileSystem fs, string path)
        {
            byte[] imageBytes;
            var fileSystem = _fileSystemProviderRepository.GetFileSystem();
            if (fileSystem != null)
            {
                if (fileSystem.Type.Contains("PhysicalFileSystem"))
                    path = fs.GetRelativePath(tImage.AbsoluteUrl);
            }

            using (var file = fs.OpenFile(path))
            {
                imageBytes = SolutionExtensions.ReadFully(file);
            }

            return imageBytes;
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
