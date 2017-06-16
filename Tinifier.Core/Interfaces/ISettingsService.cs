using Tinifier.Core.Models;

namespace Tinifier.Core.Interfaces
{
    public interface ISettingsService
    {
        void CreateSettings(TSetting setting);

        TSetting GetSettings();
    }
}
