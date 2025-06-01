using Askarr.WebApi.AskarrBot.DownloadClients.Lidarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Ombi;
using Askarr.WebApi.AskarrBot.DownloadClients.Overseerr;
using Askarr.WebApi.AskarrBot.DownloadClients.Radarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Sonarr;

namespace Askarr.WebApi.config
{
    public class DownloadClientsSettings
    {
        public OmbiSettings Ombi { get; set; }
        public OverseerrSettings Overseerr { get; set; }
        public RadarrSettings Radarr { get; set; }
        public SonarrSettings Sonarr { get; set; }

        public LidarrSettings Lidarr { get; set; }
    }
}