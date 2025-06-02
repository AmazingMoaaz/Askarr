using  Askarr.WebApi.config;
using  Askarr.WebApi. AskarrBot;
using  Askarr.WebApi. AskarrBot.DownloadClients.Lidarr;
using  Askarr.WebApi. AskarrBot.DownloadClients.Ombi;
using  Askarr.WebApi. AskarrBot.DownloadClients.Overseerr;
using  Askarr.WebApi. AskarrBot.DownloadClients.Radarr;
using  Askarr.WebApi. AskarrBot.DownloadClients.Sonarr;

namespace  Askarr.WebApi.Controllers.DownloadClients
{
    public class DownloadClientsSettingsProvider
    {
        public DownloadClientsSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new DownloadClientsSettings
            {
                Ombi = new OmbiSettingsProvider().Provide(),
                Overseerr = new OverseerrSettingsProvider().Provide(),
                Radarr = new RadarrSettingsProvider().Provide(),
                Sonarr = new SonarrSettingsProvider().Provide(),
                Lidarr = new LidarrSettingsProvider().Provider()
            };
        }
    }
}