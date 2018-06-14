using System.Collections.Generic;
using uMedia = Umbraco.Core.Models.Media;

namespace Tinifier.Core.Services.Media.Organizers
{
    interface IMediaHistoryService
    {
        /// <summary>
        /// Backup media paths before they be moved
        /// </summary>
        /// <param name="media">Media to move</param>
        void BackupMediaPaths(IEnumerable<uMedia> media);

        /// <summary>
        /// Place media back to the foldes where they were before the organizing
        /// </summary>
        void DiscardOrganizing();
    }
}
