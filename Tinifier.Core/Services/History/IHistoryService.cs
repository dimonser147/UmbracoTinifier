using System.Collections.Generic;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.History
{
    public interface IHistoryService
    {
        // Create History and save in database
        void CreateResponseHistoryItem(int timageId, TinyResponse responseItem);

        /// <summary>
        ///  Get History for Image
        /// </summary>
        /// <param name="timageId"></param>
        /// <returns></returns>
        TinyPNGResponseHistory GetImageHistory(int timageId);

        // Get not optimized Images that don`t have history
        List<TImage> CheckImageHistory(IEnumerable<TImage> images);
    }
}
