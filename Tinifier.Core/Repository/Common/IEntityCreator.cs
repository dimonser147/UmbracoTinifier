namespace Tinifier.Core.Repository.Common
{
    /// <summary>
    /// Entity creator repository
    /// </summary>
    /// <typeparam name="TEntity">class</typeparam>
    public interface IEntityCreator<TEntity> where TEntity : class
    {
        void Create(TEntity entity);
    }
}
