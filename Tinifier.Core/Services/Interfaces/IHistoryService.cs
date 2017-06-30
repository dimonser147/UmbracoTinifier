using System.Collections.Generic;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.Interfaces
{
    public interface IHistoryService
    {
        // Create History and save in database
        void CreateResponseHistoryItem(int timageId, TinyResponse responseItem);

        // Get History for Image
        TinyPNGResponseHistory GetHistoryForImage(int timageId);

        // Get not optimized Images that don`t have history
        List<TImage> CheckImageHistory(IEnumerable<TImage> images);
    }
}
