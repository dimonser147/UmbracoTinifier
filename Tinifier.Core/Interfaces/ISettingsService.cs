using Tinifier.Core.Models;

namespace Tinifier.Core.Interfaces
{
    public interface ISettingsService
    {
        // Add settings to database
        void CreateSettings(TSetting setting);

        // Get settings for displaying
        TSetting GetSettings();
    }
}
