using Askarr.WebApi.config;
using Askarr.WebApi.AskarrBot;

namespace Askarr.WebApi.Controllers.Authentication
{
    public class AuthenticationSettingsProvider
    {
        public AuthenticationSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new AuthenticationSettings
            {
                Username = (string)settings.Authentication.Username,
                Password = (string)settings.Authentication.Password,
                PrivateKey = (string)settings.Authentication.PrivateKey,
            };
        }
    }
}
