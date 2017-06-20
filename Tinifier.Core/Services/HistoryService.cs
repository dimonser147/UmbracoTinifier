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
            TinyPNGResponseHistory newItem = new TinyPNGResponseHistory
            {
                OccuredAt = DateTime.UtcNow,
                IsOptimized = responseItem.Output.IsOptimized,
                ImageId = timageId,
                Error = responseItem.Output.Error,
                Ratio = responseItem.Output.Ratio,
                OptimizedSize = responseItem.Output.Size
            };

            if (responseItem.Input != null)
            {
                newItem.OriginSize = responseItem.Input.Size;
                newItem.Error = string.Empty;
            }

            _historyRepository.Create(newItem);
        }

        public TinyPNGResponseHistory GetHistoryForImage(int timageId)
        {
            var history = _historyRepository.GetByKey(timageId);

            return history;
        }
    }
}
