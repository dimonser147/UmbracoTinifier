namespace Tinifier.Core.Repository.Interfaces
{
    // Repository for settings with custom methods
    public interface ISettingsRepository<TEntity> where TEntity : class
    {
        TEntity GetSettings();

        void Update(int currentMonthRequests);
    }
}
