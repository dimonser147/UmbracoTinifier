using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.ImageCropperInfo
{
    public interface IImageCropperInfoService
    {
        TImageCropperInfo Get(string key);

        void Create(string key, string imageId);

        void Delete(string key);

        void Update(string key, string imageId);
    }
}
