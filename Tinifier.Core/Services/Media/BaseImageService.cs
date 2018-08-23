using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.Db;
using Umbraco.Core.IO;
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
                Id = uMedia.Id.ToString(),
                Name = uMedia.Name,
                AbsoluteUrl = SolutionExtensions.GetAbsoluteUrl(uMedia)
            };
        }

        /// <summary>
        /// Update physical media file, method depens on FileSystemProvider
        /// </summary>
        /// <param name="image"></param>
        /// <param name="optimizedImageBytes"></param>
        protected void UpdateMedia(TImage image, byte[] optimizedImageBytes)
        {
            var path = FileSystem.GetRelativePath(image.AbsoluteUrl);
            //if (!FileSystem.FileExists(path))
            //    throw new InvalidOperationException("Physical media file doesn't exist in " + path);
            using (Stream stream = new MemoryStream(optimizedImageBytes))
            {
                FileSystem.AddFile(path, stream, true);
            }     
        }
    }
}
