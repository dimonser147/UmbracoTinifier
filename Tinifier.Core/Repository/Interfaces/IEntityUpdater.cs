namespace Tinifier.Core.Repository.Interfaces
{
    // Entity updater repository
    public interface IEntityUpdater<TEntity> where TEntity : class
    {
        void Update(TEntity entity);
    }
}
