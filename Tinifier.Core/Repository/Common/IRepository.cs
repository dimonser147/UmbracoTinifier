namespace Tinifier.Core.Repository.Common
{
    /// <summary>
    /// Full repository with read-write options
    /// </summary>
    /// <typeparam name="TEntity">class</typeparam>
    public interface IRepository<TEntity> : IEntityCreator<TEntity>, IEntityUpdater<TEntity>, IEntityReader<TEntity> where TEntity : class
    {
    }
}
