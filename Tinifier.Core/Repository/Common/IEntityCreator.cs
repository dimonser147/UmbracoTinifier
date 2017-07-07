namespace Tinifier.Core.Repository.Common
{
    // Entity creator repository
    public interface IEntityCreator<TEntity> where TEntity : class
    {
        void Create(TEntity entity);
    }
}
