namespace Tinifier.Core.Repository.Common
{
    // Entity updater repository
    public interface IEntityUpdater<TEntity> where TEntity : class
    {
        void Update(TEntity entity);
    }
}
