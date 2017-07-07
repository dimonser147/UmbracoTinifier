using Tinifier.Core.Models.Db;

namespace Tinifier.Core.Services.Settings
{
    public interface ISettingsService
    {
        // Add settings to database
        void CreateSettings(TSetting setting);

        // Get settings for displaying
        TSetting GetSettings();

        // Check if user has settings and ApiKey
        void CheckIfSettingExists();

        // Update number of available requests
        void UpdateSettings(int currentMonthRequests);
    }
}
