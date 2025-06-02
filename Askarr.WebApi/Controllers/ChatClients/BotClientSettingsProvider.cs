using  Askarr.WebApi.config;
using  Askarr.WebApi. AskarrBot;

namespace  Askarr.WebApi.Controllers.ChatClients
{
    public class BotClientSettingsProvider
    {
        public BotClientSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new BotClientSettings
            {
                Client = (string)settings.BotClient.Client,
            };
        }
    }
}