using System;
using Tinifier.Core.Interfaces;
using Tinifier.Core.Models;
using Tinifier.Core.Models.API;
using Tinifier.Core.Repository;

namespace Tinifier.Core.Services
{
    public class HistoryService : IHistoryService
    {
        private THistoryRepository _historyRepository;

        public HistoryService()
        {
            _historyRepository = new THistoryRepository();  
        }

        public void CreateResponseHistoryItem(int timageId, TinyResponse responseItem)
        {
            TinyPNGResponseHistory newItem = new TinyPNGResponseHistory();
            newItem.OccuredAt = DateTime.Now;
            newItem.IsOptimized = responseItem.Output.IsOptimized;
            newItem.ImageId = timageId;
            newItem.Error = responseItem.Output.Error;
            newItem.Ratio = responseItem.Output.Ratio;
            newItem.OptimizedSize = responseItem.Output.Size;

            if (responseItem.Input != null)
            {
                newItem.OriginSize = responseItem.Input.Size;
                newItem.Error = string.Empty;
            }

            _historyRepository.CreateItem(newItem);
        }

        public TinyPNGResponseHistory GetHistoryForImage(int timageId)
        {
            var history = _historyRepository.GetItemById(timageId);

            return history;
        }
    }
}
