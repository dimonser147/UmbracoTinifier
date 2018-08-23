using System.IO;
using System.Web.Hosting;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.History;

namespace Tinifier.Core.Services.History
{
    public class TImageHistoryService : IImageHistoryService
    {
        private readonly TImageHistoryRepository _imageHistoryRepository;

        public TImageHistoryService()
        {
            _imageHistoryRepository = new TImageHistoryRepository();
        }

        public void Create(TImage image, byte[] originImage)
        {
            var directoryPath = HostingEnvironment.MapPath(PackageConstants.TinifierTempFolder);
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, image.Name);
            File.WriteAllBytes(filePath, originImage);

            var model = new TinifierImagesHistory
            {
                ImageId = image.Id,
                OriginFilePath = filePath
            };

            _imageHistoryRepository.Create(model);
        }

        public TinifierImagesHistory Get(int id)
        {
            return _imageHistoryRepository.Get(id);
        }

        public void Delete(int id)
        {
            _imageHistoryRepository.Delete(id);
        }
    }
}
