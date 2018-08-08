using Tinifier.Core.Models.Db;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Tinifier.Core.Repository.ImageCropperInfo
{
    public class TImageCropperInfoRepository : IImageCropperInfoRepository
    {
        private readonly UmbracoDatabase _database;

        public TImageCropperInfoRepository()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }

        public TImageCropperInfo Get(string key)
        {
            var query = new Sql("SELECT * FROM TinifierImageCropperInfo WHERE [Key] = @0", key);
            return _database.FirstOrDefault<TImageCropperInfo>(query);
        }

        public void Create(string key, string imageId)
        {
            var info = new TImageCropperInfo
            {
                Key = key,
                ImageId = imageId
            };

            _database.Insert(info);
        }

        public void Delete(string key)
        {
            var query = new Sql("DELETE FROM TinifierImageCropperInfo WHERE [Key] = @0", key);
            _database.Execute(query);
        }

        public void Update(string key, string imageId)
        {
            var query = new Sql("UPDATE TinifierImageCropperInfo SET ImageId = @0 WHERE [Key] = @1", imageId, key);
            _database.Execute(query);
        }
    }
}
