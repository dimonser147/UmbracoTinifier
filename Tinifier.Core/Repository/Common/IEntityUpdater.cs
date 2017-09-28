namespace Tinifier.Core.Repository.Common
{
    /// <summary>
    /// Entity updater repository
    /// </summary>
    /// <typeparam name="TEntity">class</typeparam>
    public interface IEntityUpdater<TEntity> where TEntity : class
    {
        void Update(TEntity entity);
    }
}
