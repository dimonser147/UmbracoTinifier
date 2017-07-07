using System.Collections.Generic;

namespace Tinifier.Core.Repository.Common
{
    // Entity reader repository
    public interface IEntityReader<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();

        TEntity GetByKey(int Id);
    }
}
