namespace Tinifier.Core.Repository.Settings
{
    /// <summary>
    /// Repository for settings with custom methods
    /// </summary>
    /// <typeparam name="TSetting">class</typeparam>
    public interface ISettingsRepository<TSetting>
    {
        TSetting GetSettings();

        void Update(int currentMonthRequests);
    }
}
