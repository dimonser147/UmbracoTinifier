using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Web;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.ImageCropperInfo;
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

        public ImageCropperInfoService()
        {
            _imageCropperInfoRepository = new TImageCropperInfoRepository();
            _historyService = new HistoryService();
            _statisticService = new StatisticService();
            _imageService = new ImageService();
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

        public void UpdateCropperFileInfo(string key, string path, string pathForFolder)
        {
            Update(key, path);
            _statisticService.UpdateStatistic();
        }

        public void SaveCropperFileInfo(string key, string path)
        {
            Create(key, path);
            _statisticService.UpdateStatistic();
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
                GetFilesAndTinify(pathForFolder);
                
            //Cropped file was Updated
            if (imageCropperInfo != null && imagePath != null)
                UpdateCropperFileInfo(key, path, pathForFolder);

            //Cropped file was Created
            if (imageCropperInfo == null && imagePath != null)
                SaveCropperFileInfo(key, path);
        }
    }
}
