using System.Collections.Generic;
using System.Linq;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.API;
using umbraco;
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
            foreach (var media in _previewModel)
            {
                int parentFolderId = _sourceFolderId;
                string year = media.DestinationPath[0];
                string month = media.DestinationPath[1];

                foreach (var folderName in media.DestinationPath)
                {
                    if (folderName == year)
                    {
                        //check if year folder exists
                        var yearFolder = uQuery.GetMediaByName(folderName).FirstOrDefault();
                        if (yearFolder == null)
                        {
                            var folderMedia = _mediaService.CreateMediaWithIdentity(folderName, parentFolderId, PackageConstants.FolderAlias);
                            parentFolderId = folderMedia.Id;
                        }
                        else
                        {
                            parentFolderId = yearFolder.Id;
                        }
                    }

                    if (folderName == month)
                    {
                        //check if month folder exists
                        var monthFolder = uQuery.GetMediaByName(folderName).FirstOrDefault();
                        var yearFolder = uQuery.GetMediaByName(year).FirstOrDefault();
                        if (yearFolder != null)
                        {
                            if (monthFolder == null)
                            {
                                var folderMedia = _mediaService.CreateMediaWithIdentity(folderName, yearFolder.Id, PackageConstants.FolderAlias);
                                parentFolderId = folderMedia.Id;
                            }
                            else
                            {
                                if (monthFolder.ParentId == yearFolder.Id)
                                {
                                    parentFolderId = monthFolder.Id;
                                }
                                else
                                {
                                    var folderMedia = _mediaService.CreateMediaWithIdentity(folderName, parentFolderId, PackageConstants.FolderAlias);
                                    parentFolderId = folderMedia.Id;
                                }
                            }
                        }
                        else
                        {
                            if (monthFolder == null)
                            {
                                var folderMedia = _mediaService.CreateMediaWithIdentity(folderName, parentFolderId, PackageConstants.FolderAlias);
                                parentFolderId = folderMedia.Id;
                            }
                            else
                            {
                                parentFolderId = monthFolder.Id;
                            }
                        }
                    }
                }
                _imageService.Move(media.Media, parentFolderId);
            }
        }
    }
}
