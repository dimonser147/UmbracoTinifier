using System.Globalization;
using Tinifier.Core.Models;
using Tinifier.Core.Models.API;

namespace Tinifier.Core.Services.Media.Organizers
{
    public class ByUploadedDateImageOrganizer : ImageOrganizer
    {
        public ByUploadedDateImageOrganizer(int folderId) : base(folderId)
        {
        }

        protected override void PreparePreviewModel()
        {
            foreach (var m in _media)
            {
                var creationDate = m.CreateDate.ToLocalTime();
                string year = creationDate.Year.ToString();
                string month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(creationDate.Month);

                _previewModel.Add(new OrganisableMediaModel
                {
                    Media = m,
                    DestinationPath = new string[] { year, month }
                });
            };
        }

        protected override void CheckConstraints()
        {
            if (_mediaHistoryService.IsFolderChildOfOrganizedFolder(_sourceFolderId))
                throw new OrganizationConstraintsException(@"This folder is already organized.");
        }
    }
}
