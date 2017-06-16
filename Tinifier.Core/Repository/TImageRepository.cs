using System;
using System.Collections.Generic;
using Tinifier.Core.Repository;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Tinifier.Core
{
    public class TImageRepository : IRepository<Media>
    {
        public IEnumerable<Media> GetAllItems()
        {
            var mediaService = ApplicationContext.Current.Services.MediaService;
            var mediaItems = mediaService.GetMediaOfMediaType(ApplicationContext.Current.Services.ContentTypeService.GetMediaType("Image").Id) as IEnumerable<Media>;

            return mediaItems;
        }

        public Media GetItemById(int Id)
        {
            var mediaService = ApplicationContext.Current.Services.MediaService;
            var mediaItem = mediaService.GetById(Id) as Media;

            return mediaItem;
        }

        public void UpdateItem(IMediaService mediaService, IMedia mediaItem)
        {
            mediaItem.UpdateDate = DateTime.Now;
            mediaService.Save(mediaItem);
        }
    }
}
