namespace Tinifier.Core.Repository.Interfaces
{
    // Entity creator repository
    public interface IEntityCreator<TEntity> where TEntity : class
    {
        void Create(TEntity entity);
    }
}
