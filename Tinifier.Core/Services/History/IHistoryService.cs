using System.Collections.Generic;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.History
{
    public interface IHistoryService
    {
        /// <summary>
        /// Create History and save in database
        /// </summary>
        /// <param name="timageId">Image Id</param>
        /// <param name="responseItem">Response from TinyPNG</param>
        void CreateResponseHistoryItem(int timageId, TinyResponse responseItem);

        /// <summary>
        ///  Get History for Image
        /// </summary>
        /// <param name="timageId">Image Id</param>
        /// <returns>TinyPNGResponseHistory</returns>
        TinyPNGResponseHistory GetImageHistory(int timageId);

        /// <summary>
        /// Get not optimized Images that don`t have history
        /// </summary>
        /// <param name="images">List of Images that need to be sorted</param>
        /// <returns>List<TImage></returns>
        List<TImage> GetImagesWithoutHistory(IEnumerable<TImage> images);
    }
}
