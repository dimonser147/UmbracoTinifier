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
        private IMediaService _mediaService;

        public TImageRepository()
        {
            _mediaService = ApplicationContext.Current.Services.MediaService;
        }

        public IEnumerable<Media> GetAllItems()
        {
            var mediaList = new List<Media>();
            var mediaItems = _mediaService.GetMediaOfMediaType(ApplicationContext.Current.Services.ContentTypeService.GetMediaType("image").Id);

            foreach(var item in mediaItems)
            {
                mediaList.Add(item as Media);
            }

            return mediaList;
        }

        public Media GetItemById(int Id)
        {
            var mediaItem = _mediaService.GetById(Id) as Media;

            return mediaItem;
        }

        public void UpdateItem(IMediaService mediaService, IMedia mediaItem)
        {
            mediaItem.UpdateDate = DateTime.Now;
            mediaService.Save(mediaItem);
        }
    }
}
