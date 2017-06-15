using System.Collections.Generic;
using Tinifier.Core.Models;

namespace Tinifier.Core.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAllItems();

        T GetItemById(int Id);

        void UpdateItem(TImage image, byte[] bytesArray);
    }
}
