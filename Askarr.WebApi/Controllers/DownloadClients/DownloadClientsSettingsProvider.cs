using Askarr.WebApi.config;
using Askarr.WebApi.AskarrBot;
using Askarr.WebApi.AskarrBot.DownloadClients.Lidarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Ombi;
using Askarr.WebApi.AskarrBot.DownloadClients.Overseerr;
using Askarr.WebApi.AskarrBot.DownloadClients.Radarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Sonarr;

namespace Askarr.WebApi.Controllers.DownloadClients
{
    public class DownloadClientsSettingsProvider
    {
        public DownloadClientsSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            var ombiSettings = new OmbiSettingsProvider().Provide();
            var overseerrSettings = new OverseerrSettingsProvider().Provide();
            var radarrSettings = new RadarrSettingsProvider().Provide();
            var sonarrSettings = new SonarrSettingsProvider().Provide();
            var lidarrSettings = new LidarrSettingsProvider().Provide();

            return new DownloadClientsSettings
            {
                Ombi = ombiSettings.ToConfigSettings(),
                Overseerr = overseerrSettings.ToConfigSettings(),
                Radarr = radarrSettings.ToConfigSettings(),
                Sonarr = sonarrSettings.ToConfigSettings(),
                Lidarr = lidarrSettings.ToConfigSettings()
            };
        }
    }
}