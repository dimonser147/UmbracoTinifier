using System.Collections.Generic;
using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Repository.History
{
    public interface IHistoryRepository
    {
        void Delete(string imageId);

        TinyPNGResponseHistory Get(string id);

        IEnumerable<TinyPNGResponseHistory> GetHistoryByPath(string path);
    }
}
