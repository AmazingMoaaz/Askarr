using System;
using System.Linq;

namespace Askarr.WebApi.config
{
    public static class SettingsConverters
    {
        public static AskarrBot.DownloadClients.Overseerr.OverseerrSettings ToOverseerrBotSettings(this OverseerrSettings settings)
        {
            return new AskarrBot.DownloadClients.Overseerr.OverseerrSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                ApiKey = settings.ApiKey,
                UseSSL = settings.UseHttps,
                // Add any other required properties here
            };
        }

        public static OverseerrSettings ToConfigSettings(this AskarrBot.DownloadClients.Overseerr.OverseerrSettings settings)
        {
            return new OverseerrSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                ApiKey = settings.ApiKey,
                UseHttps = settings.UseSSL,
                // Add any other required properties here
            };
        }

        public static AskarrBot.DownloadClients.Ombi.OmbiSettings ToOmbiBotSettings(this OmbiSettings settings)
        {
            return new AskarrBot.DownloadClients.Ombi.OmbiSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                ApiKey = settings.ApiKey,
                ApiUsername = settings.ApiUsername,
                UseSSL = settings.UseSSL,
                // Add any other required properties here
            };
        }

        public static OmbiSettings ToConfigSettings(this AskarrBot.DownloadClients.Ombi.OmbiSettings settings)
        {
            return new OmbiSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                ApiKey = settings.ApiKey,
                ApiUsername = settings.ApiUsername,
                UseSSL = settings.UseSSL,
                // Add any other required properties here
            };
        }

        public static AskarrBot.DownloadClients.Radarr.RadarrSettings ToRadarrBotSettings(this RadarrSettings settings)
        {
            return new AskarrBot.DownloadClients.Radarr.RadarrSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                ApiKey = settings.ApiKey,
                UseSSL = settings.UseSSL,
                // Add any other required properties here
            };
        }

        public static RadarrSettings ToConfigSettings(this AskarrBot.DownloadClients.Radarr.RadarrSettings settings)
        {
            return new RadarrSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                ApiKey = settings.ApiKey,
                UseSSL = settings.UseSSL,
                // Add any other required properties here
            };
        }

        public static AskarrBot.DownloadClients.Sonarr.SonarrSettings ToSonarrBotSettings(this SonarrSettings settings)
        {
            return new AskarrBot.DownloadClients.Sonarr.SonarrSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                ApiKey = settings.ApiKey,
                UseSSL = settings.UseSSL,
                // Add any other required properties here
            };
        }

        public static SonarrSettings ToConfigSettings(this AskarrBot.DownloadClients.Sonarr.SonarrSettings settings)
        {
            return new SonarrSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                ApiKey = settings.ApiKey,
                UseSSL = settings.UseSSL,
                // Add any other required properties here
            };
        }

        public static AskarrBot.DownloadClients.Lidarr.LidarrSettings ToLidarrBotSettings(this LidarrSettings settings)
        {
            return new AskarrBot.DownloadClients.Lidarr.LidarrSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                ApiKey = settings.ApiKey,
                UseSSL = settings.UseSSL,
                // Add any other required properties here
            };
        }

        public static LidarrSettings ToConfigSettings(this AskarrBot.DownloadClients.Lidarr.LidarrSettings settings)
        {
            return new LidarrSettings
            {
                Hostname = settings.Hostname,
                Port = settings.Port,
                ApiKey = settings.ApiKey,
                UseSSL = settings.UseSSL,
                // Add any other required properties here
            };
        }
    }
} 