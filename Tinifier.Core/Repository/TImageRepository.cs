using System;
using System.Collections.Generic;
using System.IO;
using Tinifier.Core.Models;
using Tinifier.Core.Repository;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Tinifier.Core
{
    public class TImageRepository : IRepository<TImage>
    {
        public IEnumerable<TImage> GetAllItems()
        {
            List<TImage> images = new List<TImage>();
            var imagesMedia = uQuery.GetMediaByType("Image");

            foreach (var item in imagesMedia)
            {
                var image = new TImage
                {
                    Id = item.Id,
                    Name = item.Text,
                    Url = item.GetImageUrl()
                };

                images.Add(image);
            }

            return images;
        }

        public TImage GetItemById(int Id)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var image = umbracoHelper.Media(Id);

            var tImage = new TImage
            {
                Id = Id,
                Name = image.Name,
                Url = image.Url
            };

            return tImage;
        }

        public void UpdateItem(TImage image, byte[] bytesArray)
        {
            var mediaService = ApplicationContext.Current.Services.MediaService;
            IMedia mediaItem = mediaService.GetById(image.Id);

            using (Stream stream = new MemoryStream(bytesArray))
            {
                mediaService.SetMediaFileContent(image.Url, stream);
            }
           
            mediaItem.UpdateDate = DateTime.Now;
            mediaService.Save(mediaItem);
        }
    }
}
