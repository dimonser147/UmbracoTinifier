using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.ImageCropperInfo;

namespace Tinifier.Core.Services.ImageCropperInfo
{
    public class ImageCropperInfoService : IImageCropperInfoService
    {
        private readonly TImageCropperInfoRepository _imageCropperInfoRepository;

        public ImageCropperInfoService()
        {
            _imageCropperInfoRepository = new TImageCropperInfoRepository();
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
    }
}
