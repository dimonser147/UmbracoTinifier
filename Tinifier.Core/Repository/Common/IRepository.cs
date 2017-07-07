namespace Tinifier.Core.Repository.Common
{
    // Full repository with read-write options
    public interface IRepository<TEntity> : IEntityCreator<TEntity>, IEntityUpdater<TEntity>, IEntityReader<TEntity> where TEntity : class
    {
    }
}
