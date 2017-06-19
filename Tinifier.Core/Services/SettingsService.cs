using Tinifier.Core.Interfaces;
using Tinifier.Core.Models;
using Tinifier.Core.Repository;

namespace Tinifier.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private TSettingsRepository _settingsRepository;

        public SettingsService()
        {
            _settingsRepository = new TSettingsRepository();
        }

        public void CreateSettings(TSetting setting)
        {
            _settingsRepository.Create(setting);
        }

        public TSetting GetSettings()
        {
            var setting = _settingsRepository.GetSettings();

            return setting;
        }
    }
}
