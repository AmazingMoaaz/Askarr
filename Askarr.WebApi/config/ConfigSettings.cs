using System;

namespace Askarr.WebApi.config
{
    public class ApplicationSettings
    {
        public bool DisableAuthentication { get; set; } = false;
        public int Port { get; set; } = 5000;
        public string BaseUrl { get; set; } = "";
    }

    public class AuthenticationSettings
    {
        public string PrivateKey { get; set; }
        public string AdminUsername { get; set; }
        public string Username { get; set; }
        public string AdminPassword { get; set; }
        public string Password { get; set; }
    }

    public class BotClientSettings
    {
        public string Client { get; set; } = "Discord";
    }

    public class ChatClientsSettings
    {
        public DiscordSettings Discord { get; set; } = new DiscordSettings();
        public TelegramSettings Telegram { get; set; } = new TelegramSettings();
        public string Language { get; set; } = "english";
    }

    public class DiscordSettings
    {
        public string BotToken { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string StatusMessage { get; set; } = "";
        public string[] TvShowRoles { get; set; } = Array.Empty<string>();
        public string[] MovieRoles { get; set; } = Array.Empty<string>();
        public string[] MusicRoles { get; set; } = Array.Empty<string>();
        public string[] MonitoredChannels { get; set; } = Array.Empty<string>();
        public bool EnableRequestsThroughDirectMessages { get; set; } = true;
        public bool AutomaticallyNotifyRequesters { get; set; } = true;
        public string NotificationMode { get; set; } = "PrivateMessages";
        public string[] NotificationChannels { get; set; } = Array.Empty<string>();
        public bool AutomaticallyPurgeCommandMessages { get; set; } = true;
    }

    public class TelegramSettings
    {
        public string BotToken { get; set; } = "";
        public string Username { get; set; } = "";
        public string[] TvShowRoles { get; set; } = Array.Empty<string>();
        public string[] MovieRoles { get; set; } = Array.Empty<string>();
        public string[] MusicRoles { get; set; } = Array.Empty<string>();
        public string[] MonitoredChats { get; set; } = Array.Empty<string>();
        public bool EnableRequestsThroughDirectMessages { get; set; } = true;
        public bool AutomaticallyNotifyRequesters { get; set; } = true;
        public string NotificationMode { get; set; } = "PrivateMessages";
        public string[] NotificationChats { get; set; } = Array.Empty<string>();
    }

    public class DownloadClientsSettings
    {
        public RadarrSettings Radarr { get; set; } = new RadarrSettings();
        public SonarrSettings Sonarr { get; set; } = new SonarrSettings();
        public OmbiSettings Ombi { get; set; } = new OmbiSettings();
        public OverseerrSettings Overseerr { get; set; } = new OverseerrSettings();
        public LidarrSettings Lidarr { get; set; } = new LidarrSettings();
    }

    public class RadarrSettings
    {
        public string Hostname { get; set; } = "";
        public int Port { get; set; } = 7878;
        public string ApiKey { get; set; } = "";
        public bool UseHttps { get; set; } = false;
        public string BaseUrl { get; set; } = "";
        public bool UseSSL { get; set; } = false;
        public string[] Categories { get; set; } = Array.Empty<string>();
        public bool SearchNewRequests { get; set; } = true;
        public bool MonitorNewRequests { get; set; } = true;
        public string Version { get; set; } = "v3";
    }

    public class SonarrSettings
    {
        public string Hostname { get; set; } = "";
        public int Port { get; set; } = 8989;
        public string ApiKey { get; set; } = "";
        public bool UseHttps { get; set; } = false;
        public string BaseUrl { get; set; } = "";
        public bool UseSSL { get; set; } = false;
        public string[] Categories { get; set; } = Array.Empty<string>();
        public bool SearchNewRequests { get; set; } = true;
        public bool MonitorNewRequests { get; set; } = true;
        public string Version { get; set; } = "v3";
    }

    public class OmbiSettings
    {
        public string Hostname { get; set; } = "";
        public int Port { get; set; } = 5000;
        public string ApiKey { get; set; } = "";
        public string ApiUsername { get; set; } = "";
        public bool UseHttps { get; set; } = false;
        public string BaseUrl { get; set; } = "";
        public bool UseSSL { get; set; } = false;
        public string Version { get; set; } = "v4";
        public bool UseMovieIssue { get; set; } = false;
        public bool UseTVIssue { get; set; } = false;
    }

    public class OverseerrSettings
    {
        public string Hostname { get; set; } = "";
        public int Port { get; set; } = 5055;
        public string ApiKey { get; set; } = "";
        public bool UseHttps { get; set; } = false;
        public OverseerrMovieSettings Movies { get; set; } = new OverseerrMovieSettings();
        public OverseerrTvShowSettings TvShows { get; set; } = new OverseerrTvShowSettings();
    }

    public class OverseerrMovieSettings
    {
        public int DefaultApiUserID { get; set; } = 1;
        public string[] Categories { get; set; } = Array.Empty<string>();
    }

    public class OverseerrTvShowSettings
    {
        public int DefaultApiUserID { get; set; } = 1;
        public string[] Categories { get; set; } = Array.Empty<string>();
    }

    public class LidarrSettings
    {
        public string Hostname { get; set; } = "";
        public int Port { get; set; } = 8686;
        public string ApiKey { get; set; } = "";
        public bool UseHttps { get; set; } = false;
        public string BaseUrl { get; set; } = "";
        public bool UseSSL { get; set; } = false;
        public string[] Categories { get; set; } = Array.Empty<string>();
        public bool SearchNewRequests { get; set; } = true;
        public bool MonitorNewRequests { get; set; } = true;
        public string Version { get; set; } = "v1";
    }
} 