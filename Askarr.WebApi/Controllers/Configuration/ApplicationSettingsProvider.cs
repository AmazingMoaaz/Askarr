using  Askarr.WebApi. AskarrBot;
using  Askarr.WebApi.config;

namespace  Askarr.WebApi.Controllers.Configuration
{
    public class ApplicationSettingsProvider
    {
        public ApplicationSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new ApplicationSettings
            {
                Port = (int)settings.Port,
                BaseUrl = (string)settings.BaseUrl,
                DisableAuthentication = (bool)settings.DisableAuthentication,
            };
        }
    }
}