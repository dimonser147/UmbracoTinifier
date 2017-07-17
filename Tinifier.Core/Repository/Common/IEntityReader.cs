using System.Collections.Generic;

namespace Tinifier.Core.Repository.Common
{
    /// <summary>
    /// Entity reader repository
    /// </summary>
    /// <typeparam name="TEntity">class</typeparam>
    public interface IEntityReader<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();

        TEntity Get(int id);
    }
}
