namespace Tinifier.Core.Repository.Common
{
    public interface IEntityRemover<TEntity> where TEntity : class
    {
        void Delete(int Id);
    }
}
