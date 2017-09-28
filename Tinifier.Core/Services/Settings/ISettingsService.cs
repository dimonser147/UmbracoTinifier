using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.Settings
{
    public interface ISettingsService
    {
        /// <summary>
        /// Add settings to database
        /// </summary>
        /// <param name="setting">TSetting</param>
        void CreateSettings(TSetting setting);

        /// <summary>
        /// Get settings for displaying
        /// </summary>
        /// <returns>TSetting</returns>
        TSetting GetSettings();

        /// <summary>
        /// Check if user has settings and ApiKey
        /// </summary>
        void CheckIfSettingExists();

        /// <summary>
        /// Update number of available requests
        /// </summary>
        /// <param name="currentMonthRequests">number of user requests</param>
        void UpdateSettings(int currentMonthRequests);
    }
}
