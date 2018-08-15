using System.Collections.Generic;
using Tinifier.Core.Models.API;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Models.Services;

namespace Tinifier.Core.Services.History
{
    public interface IHistoryService
    {
        /// <summary>
        /// Create History and save in database
        /// </summary>
        /// <param name="timageId">Image Id</param>
        /// <param name="responseItem">Response from TinyPNG</param>
        void CreateResponseHistory(string timageId, TinyResponse responseItem);

        /// <summary>
        ///  Get History for Image
        /// </summary>
        /// <param name="timageId">Image Id</param>
        /// <returns>TinyPNGResponseHistory</returns>
        TinyPNGResponseHistory GetImageHistory(string timageId);

        /// <summary>
        /// Get not optimized Images that don`t have history
        /// </summary>
        /// <param name="images">List of Images that need to be sorted</param>
        /// <returns>List<TImage></returns>
        List<TImage> GetImagesWithoutHistory(IEnumerable<TImage> images);

        /// <summary>
        /// Get histories by day for chart
        /// </summary>
        /// <returns>IEnumerable<HistoriesStatisticModel></returns>
        IEnumerable<HistoriesStatisticModel> GetStatisticByDays();

        /// <summary>
        /// Delete history for image
        /// </summary>
        /// <param name="imageId">Image Id</param>
        void Delete(string imageId);

        IEnumerable<TinyPNGResponseHistory> GetHistoryByPath(string path);
    }
}
