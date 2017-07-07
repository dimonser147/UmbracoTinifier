using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Settings;

namespace Tinifier.Core.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly TSettingsRepository _settingsRepository;

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

        public void CheckIfSettingExists()
        {
            var setting = _settingsRepository.GetSettings();

            if (setting == null)
            {
                throw new EntityNotFoundException(PackageConstants.ApiKeyNotFound);
            }
        }

        public void UpdateSettings(int currentMonthRequests)
        {
            _settingsRepository.Update(currentMonthRequests);
        }
    }
}
