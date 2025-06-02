using  Askarr.WebApi. AskarrBot;
using  Askarr.WebApi.config;

namespace  Askarr.WebApi.Controllers.Configuration
{
    public static class ApplicationSettingsRepository
    {
        public static void Update(ApplicationSettings applicationSettings)
        {
            SettingsFile.Write(settings =>
            {
                settings.Port = applicationSettings.Port;
                settings.BaseUrl = applicationSettings.BaseUrl;
                settings.DisableAuthentication = applicationSettings.DisableAuthentication;
            });
        }
    }
}