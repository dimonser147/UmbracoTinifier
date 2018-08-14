using System.IO;
using System.Linq;
using System.Web;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.FileSystemProvider;
using Tinifier.Core.Repository.ImageCropperInfo;
using Tinifier.Core.Services.BlobStorage;
using Tinifier.Core.Services.History;
using Tinifier.Core.Services.Media;
using Tinifier.Core.Services.Statistic;

namespace Tinifier.Core.Services.ImageCropperInfo
{
    public class ImageCropperInfoService : IImageCropperInfoService
    {
        private readonly TImageCropperInfoRepository _imageCropperInfoRepository;
        private readonly IHistoryService _historyService;
        private readonly IStatisticService _statisticService;
        private readonly IImageService _imageService;
        private readonly IFileSystemProviderRepository _fileSystemProviderRepository;
        private readonly IBlobStorage _blobStorage;

        public ImageCropperInfoService()
        {
            _imageCropperInfoRepository = new TImageCropperInfoRepository();
            _historyService = new HistoryService();
            _statisticService = new StatisticService();
            _imageService = new ImageService();
            _fileSystemProviderRepository = new TFileSystemProviderRepository();
            _blobStorage = new AzureBlobStorageService();
        }

        public void Create(string key, string imageId)
        {
            _imageCropperInfoRepository.Create(key, imageId);
        }

        public void Delete(string key)
        {
            _imageCropperInfoRepository.Delete(key);
        }

        public TImageCropperInfo Get(string key)
        {
            return _imageCropperInfoRepository.Get(key);
        }

        public void Update(string key, string imageId)
        {
            _imageCropperInfoRepository.Update(key, imageId);
        }

        public void GetFilesAndTinify(string pathForFolder)
        {
            var fileSystem = _fileSystemProviderRepository.GetFileSystem();

            if(fileSystem != null)
            {
                if (fileSystem.Type.Contains("PhysicalFileSystem"))
                {
                    var serverPathForFolder = HttpContext.Current.Server.MapPath(pathForFolder);
                    var di = new DirectoryInfo(serverPathForFolder);
                    var files = di.GetFiles();

                    foreach (var file in files)
                    {
                        var image = new TImage
                        {
                            Id = Path.Combine(pathForFolder, file.Name),
                            Name = file.Name,
                            AbsoluteUrl = Path.Combine(pathForFolder, file.Name)
                        };

                        var imageHistory = _historyService.GetImageHistory(image.Id);
                        if (imageHistory != null && imageHistory.IsOptimized)
                            continue;

                        _imageService.OptimizeImageAsync(image).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    var blobs = _blobStorage.GetAllBlobsInContainer();
                }
            }            
        }

        public void ValidateFileExtension(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new EntityNotFoundException();

            var fileExt = Path.GetExtension(path).ToUpper().Replace(".", string.Empty).Trim();
            if (!PackageConstants.SupportedExtensions.Contains(fileExt))
                throw new NotSupportedExtensionException(fileExt);
        }

        public void DeleteImageFromImageCropper(string key, TImageCropperInfo imageCropperInfo)
        {
            Delete(key);

            var pathForFolder = imageCropperInfo.ImageId.Remove(imageCropperInfo.ImageId.LastIndexOf('/') + 1);
            var histories = _historyService.GetHistoryByPath(pathForFolder);

            foreach (var history in histories)
                _historyService.Delete(history.ImageId);

            _statisticService.UpdateStatistic();
        }

        public void GetCropImagesAndTinify(string key, TImageCropperInfo imageCropperInfo, object imagePath, 
            bool enableCropsOptimization, string path)
        {
            ValidateFileExtension(path);
            var pathForFolder = path.Remove(path.LastIndexOf('/') + 1);

            var histories = _historyService.GetHistoryByPath(pathForFolder);
            foreach (var history in histories)
                _historyService.Delete(history.ImageId);

            if (enableCropsOptimization)
            {
                GetFilesAndTinify(pathForFolder);

                //Cropped file was Updated
                if (imageCropperInfo != null && imagePath != null)
                    Update(key, path);

                //Cropped file was Created
                if (imageCropperInfo == null && imagePath != null)
                    Create(key, path);
            }

            _statisticService.UpdateStatistic();
        }
    }
}
