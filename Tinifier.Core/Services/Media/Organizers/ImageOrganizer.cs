using System.Collections.Generic;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.API;
using Umbraco.Core.Services;
using uMedia = Umbraco.Core.Models.Media;

namespace Tinifier.Core.Services.Media.Organizers
{
    public abstract class ImageOrganizer
    {
        protected readonly IImageService _imageService;
        private readonly IMediaService _mediaService;
        private readonly IMediaHistoryService _mediaHistoryService;
        private readonly int _sourceFolderId;
        protected readonly IEnumerable<uMedia> _media;
        protected List<OrganisableMediaModel> _previewModel;

        public ImageOrganizer(int sourceFolderId)
        {
            _sourceFolderId = sourceFolderId;
            _previewModel = new List<OrganisableMediaModel>();
            _imageService = new ImageService();
            _mediaHistoryService = (IMediaHistoryService)_imageService;
            _media = _imageService.GetAllImagesAt(_sourceFolderId);
            _mediaService = Umbraco.Core.ApplicationContext.Current.Services.MediaService;
        }

        protected abstract void PreparePreviewModel();

        public void Organize()
        {
            PreparePreviewModel();
            SaveCurrentState();
            ProcessPreviewModel();
        }

        private void SaveCurrentState()
        {
            _mediaHistoryService.BackupMediaPaths(_media);
        }

        private void ProcessPreviewModel()
        {
            var createdDestinations = new Dictionary<string, int>();

            foreach (var media in _previewModel)
            {
                int parentFolderId = _sourceFolderId;
                string destinationPath = string.Join("/", media.DestinationPath);

                if (createdDestinations.ContainsKey(destinationPath))
                {
                    parentFolderId = createdDestinations[destinationPath];
                }
                else
                {
                    foreach (var folderName in media.DestinationPath)
                    {
                        var folderMedia =
                            _mediaService.CreateMediaWithIdentity(folderName, parentFolderId, PackageConstants.FolderAlias);
                        parentFolderId = folderMedia.Id;
                    }
                    createdDestinations.Add(destinationPath, parentFolderId);
                }

                _imageService.Move(media.Media, parentFolderId);
            }
        }
    }
}
