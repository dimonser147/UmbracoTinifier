using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.History
{
    public interface IImageHistoryService
    {
        void Create(TImage image, byte[] originImage);

        TinifierImagesHistory Get(int id);

        void Delete(int id);
    }
}
