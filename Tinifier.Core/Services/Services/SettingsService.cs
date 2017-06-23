using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Repository;
using Tinifier.Core.Services.Interfaces;

namespace Tinifier.Core.Services.Services
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
