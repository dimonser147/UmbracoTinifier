using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Repository.ImageCropperInfo
{
    public interface IImageCropperInfoRepository
    {
        TImageCropperInfo Get(string key);

        void Create(string key, string imageId);

        void Delete(string key);

        void Update(string key, string imageId);
    }
}
