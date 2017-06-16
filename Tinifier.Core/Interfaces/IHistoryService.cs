using Tinifier.Core.Models;
using Tinifier.Core.Models.API;

namespace Tinifier.Core.Interfaces
{
    public interface IHistoryService
    {
        // Create History and save in database
        void CreateResponseHistoryItem(int timageId, TinyResponse responseItem);

        // Get History for Image
        TinyPNGResponseHistory GetHistoryForImage(int timageId);
    }
}
