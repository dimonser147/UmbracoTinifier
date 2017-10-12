using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tinifier.Core.Models.Db;
using Umbraco.Core.IO;
using Umbraco.Web;
using uModels = Umbraco.Core.Models;

namespace Tinifier.Core.Services.Media
{
    public abstract class BaseImageService
    {
        public IFileSystem FileSystem { get; private set; } = FileSystemProviderManager.Current.GetFileSystemProvider<MediaFileSystem>();

        public IEnumerable<TImage> Convert(IEnumerable<uModels.Media> items)
        {
            return items.Select(x => Convert(x));
        }

        public TImage Convert(uModels.Media uMedia)
        {
            return new TImage
            {
                Id = uMedia.Id,
                Name = uMedia.Name,
                AbsoluteUrl = GetAbsoluteUrl(uMedia)
            };
        }


        /// <summary>
        /// Get absolute url of Umbraco media file
        /// </summary>
        /// <param name="uMedia"></param>
        /// <returns></returns>
        protected string GetAbsoluteUrl(uModels.Media uMedia)
        {
            var umbHelper = new UmbracoHelper(UmbracoContext.Current);
            var content = umbHelper.Media(uMedia.Id);
            var imagerUrl = content.Url;
            return imagerUrl;
        }

        /// <summary>
        /// Update physical media file, method depens on FileSystemProvider
        /// </summary>
        /// <param name="image"></param>
        /// <param name="optimizedImageBytes"></param>
        protected void UpdateMedia(TImage image, byte[] optimizedImageBytes)
        {
            string path = FileSystem.GetRelativePath(image.AbsoluteUrl);
            //if (!FileSystem.FileExists(path))
            //    throw new InvalidOperationException("Physical media file doesn't exist in " + path);
            using (Stream stream = new MemoryStream(optimizedImageBytes))
            {
                FileSystem.AddFile(path, stream, true);
            }     
        }
    }
}
