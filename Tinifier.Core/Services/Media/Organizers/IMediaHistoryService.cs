using System.Collections.Generic;
using uMedia = Umbraco.Core.Models.Media;

namespace Tinifier.Core.Services.Media.Organizers
{
    public interface IMediaHistoryService
    {
        /// <summary>
        /// Backup media paths before they be moved
        /// </summary>
        /// <param name="media">Media to move</param>
        void BackupMediaPaths(IEnumerable<uMedia> media);

        /// <summary>
        /// Place media back to the foldes where they were before the organizing
        /// </summary>
        void DiscardOrganizing(int folderId);

        /// <summary>
        /// Checks if folder is a child of already optimized folder
        /// </summary>
        /// <param name="sourceFolderId">Folder to check</param>
        /// <returns></returns>
        bool IsFolderChildOfOrganizedFolder(int folderId);
    }
}
