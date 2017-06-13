using System.Collections.Generic;

namespace Tinifier.Core.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAllItems();

        T GetItemById(int Id);
    }
}
