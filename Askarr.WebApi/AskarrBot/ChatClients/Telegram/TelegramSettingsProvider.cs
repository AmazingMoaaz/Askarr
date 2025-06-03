using System;
using System.Linq;
using Askarr.WebApi.AskarrBot.DownloadClients;
using Askarr.WebApi.AskarrBot.DownloadClients.Lidarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Ombi;
using Askarr.WebApi.AskarrBot.DownloadClients.Overseerr;
using Askarr.WebApi.AskarrBot.DownloadClients.Radarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Sonarr;
using Askarr.WebApi.AskarrBot.Extensions;
using Newtonsoft.Json.Linq;

namespace Askarr.WebApi.AskarrBot.ChatClients.Telegram
{
    public class TelegramSettingsProvider
    {
        public TelegramSettings Provide()
        {
            dynamic settings = SettingsFile.Read();
            
            // Check if ChatClients.Telegram section exists, if not, create a default structure
            if (settings.ChatClients == null)
            {
                return new TelegramSettings
                {
                    BotToken = "",
                    Username = "",
                    MonitoredChats = Array.Empty<string>(),
                    TvShowRoles = Array.Empty<string>(),
                    MovieRoles = Array.Empty<string>(),
                    MusicRoles = Array.Empty<string>(),
                    EnableRequestsThroughDirectMessages = true,
                    AutomaticallyNotifyRequesters = true,
                    NotificationMode = "PrivateMessages",
                    NotificationChats = Array.Empty<string>()
                };
            }
            
            if (settings.ChatClients.Telegram == null)
            {
                return new TelegramSettings
                {
                    BotToken = "",
                    Username = "",
                    MonitoredChats = Array.Empty<string>(),
                    TvShowRoles = Array.Empty<string>(),
                    MovieRoles = Array.Empty<string>(),
                    MusicRoles = Array.Empty<string>(),
                    EnableRequestsThroughDirectMessages = true,
                    AutomaticallyNotifyRequesters = true,
                    NotificationMode = "PrivateMessages",
                    NotificationChats = Array.Empty<string>()
                };
            }

            // Safely extract properties with null checks
            return new TelegramSettings
            {
                BotToken = settings.ChatClients.Telegram.BotToken ?? "",
                Username = settings.ChatClients.Telegram.Username ?? "",
                MovieDownloadClient = settings.Movies?.Client ?? "",
                MovieDownloadClientConfigurationHash = ComputeMovieClientConfigurationHashCode(settings),
                TvShowDownloadClient = settings.TvShows?.Client ?? "",
                TvShowDownloadClientConfigurationHash = ComputeTvClientConfigurationHashCode(settings),
                MusicDownloadClient = settings.Music?.Client ?? "",
                MusicDownloadClientConfigurationHash = ComputeMusicClientConfigurationHashCode(settings),
                MonitoredChats = GetArraySafely(settings.ChatClients.Telegram.MonitoredChats),
                TvShowRoles = GetArraySafely(settings.ChatClients.Telegram.TvShowRoles),
                MovieRoles = GetArraySafely(settings.ChatClients.Telegram.MovieRoles),
                MusicRoles = GetArraySafely(settings.ChatClients.Telegram.MusicRoles),
                EnableRequestsThroughDirectMessages = settings.ChatClients.Telegram.EnableRequestsThroughDirectMessages ?? true,
                AutomaticallyNotifyRequesters = settings.ChatClients.Telegram.AutomaticallyNotifyRequesters ?? true,
                NotificationMode = settings.ChatClients.Telegram.NotificationMode ?? "PrivateMessages",
                NotificationChats = GetArraySafely(settings.ChatClients.Telegram.NotificationChats),
            };
        }

        private string[] GetArraySafely(dynamic value)
        {
            if (value == null)
            {
                return Array.Empty<string>();
            }
            
            try
            {
                return value.ToObject<string[]>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        public static void Update(TelegramSettings telegramSettings)
        {
            if (telegramSettings == null)
            {
                throw new ArgumentNullException(nameof(telegramSettings), "Telegram settings cannot be null");
            }

            dynamic settings = SettingsFile.Read();

            // Ensure ChatClients exists
            if (settings.ChatClients == null)
            {
                settings.ChatClients = new JObject();
            }
            
            // Ensure ChatClients.Telegram exists
            if (settings.ChatClients.Telegram == null)
            {
                settings.ChatClients.Telegram = new JObject();
            }

            SettingsFile.Write(s => {
                s.ChatClients.Telegram.BotToken = telegramSettings.BotToken ?? string.Empty;
                s.ChatClients.Telegram.MonitoredChats = JToken.FromObject(telegramSettings.MonitoredChats ?? Array.Empty<string>());
                s.ChatClients.Telegram.EnableRequestsThroughDirectMessages = telegramSettings.EnableRequestsThroughDirectMessages;
                s.ChatClients.Telegram.TvShowRoles = JToken.FromObject(telegramSettings.TvShowRoles ?? Array.Empty<string>());
                s.ChatClients.Telegram.MovieRoles = JToken.FromObject(telegramSettings.MovieRoles ?? Array.Empty<string>());
                s.ChatClients.Telegram.MusicRoles = JToken.FromObject(telegramSettings.MusicRoles ?? Array.Empty<string>());
                s.ChatClients.Telegram.AutomaticallyNotifyRequesters = telegramSettings.AutomaticallyNotifyRequesters;
                s.ChatClients.Telegram.NotificationMode = telegramSettings.NotificationMode ?? "PrivateMessages";
                s.ChatClients.Telegram.NotificationChats = JToken.FromObject(telegramSettings.NotificationChats ?? Array.Empty<string>());
            });
        }

        private int ComputeMovieClientConfigurationHashCode(dynamic settings)
        {
            var hashCode = new HashCode();

            if (settings.Movies?.Client == "Radarr" && settings.DownloadClients?.Radarr != null)
            {
                hashCode.Add(settings.DownloadClients.Radarr.Hostname ?? "");
                hashCode.Add(settings.DownloadClients.Radarr.Port ?? 0);
                hashCode.Add(settings.DownloadClients.Radarr.ApiKey ?? "");
            }
            else if (settings.Movies?.Client == "Ombi" && settings.DownloadClients?.Ombi != null)
            {
                hashCode.Add(settings.DownloadClients.Ombi.Hostname ?? "");
                hashCode.Add(settings.DownloadClients.Ombi.Port ?? 0);
                hashCode.Add(settings.DownloadClients.Ombi.ApiKey ?? "");
                hashCode.Add(settings.DownloadClients.Ombi.ApiUsername ?? "");
            }
            else if (settings.Movies?.Client == "Overseerr" && settings.DownloadClients?.Overseerr != null)
            {
                hashCode.Add(settings.DownloadClients.Overseerr.Hostname ?? "");
                hashCode.Add(settings.DownloadClients.Overseerr.Port ?? 0);
                hashCode.Add(settings.DownloadClients.Overseerr.ApiKey ?? "");
                if (settings.DownloadClients.Overseerr.Movies != null)
                {
                    hashCode.Add(settings.DownloadClients.Overseerr.Movies.DefaultApiUserID ?? 0);
                }
            }

            return hashCode.ToHashCode();
        }

        private int ComputeTvClientConfigurationHashCode(dynamic settings)
        {
            var hashCode = new HashCode();

            if (settings.TvShows?.Client == "Sonarr" && settings.DownloadClients?.Sonarr != null)
            {
                hashCode.Add(settings.DownloadClients.Sonarr.Hostname ?? "");
                hashCode.Add(settings.DownloadClients.Sonarr.Port ?? 0);
                hashCode.Add(settings.DownloadClients.Sonarr.ApiKey ?? "");
            }
            else if (settings.TvShows?.Client == "Ombi" && settings.DownloadClients?.Ombi != null)
            {
                hashCode.Add(settings.DownloadClients.Ombi.Hostname ?? "");
                hashCode.Add(settings.DownloadClients.Ombi.Port ?? 0);
                hashCode.Add(settings.DownloadClients.Ombi.ApiKey ?? "");
                hashCode.Add(settings.DownloadClients.Ombi.ApiUsername ?? "");
            }
            else if (settings.TvShows?.Client == "Overseerr" && settings.DownloadClients?.Overseerr != null)
            {
                hashCode.Add(settings.DownloadClients.Overseerr.Hostname ?? "");
                hashCode.Add(settings.DownloadClients.Overseerr.Port ?? 0);
                hashCode.Add(settings.DownloadClients.Overseerr.ApiKey ?? "");
                if (settings.DownloadClients.Overseerr.TvShows != null)
                {
                    hashCode.Add(settings.DownloadClients.Overseerr.TvShows.DefaultApiUserID ?? 0);
                }
            }

            return hashCode.ToHashCode();
        }

        private int ComputeMusicClientConfigurationHashCode(dynamic settings)
        {
            var hashCode = new HashCode();

            if (settings.Music?.Client == "Lidarr" && settings.DownloadClients?.Lidarr != null)
            {
                hashCode.Add(settings.DownloadClients.Lidarr.Hostname ?? "");
                hashCode.Add(settings.DownloadClients.Lidarr.Port ?? 0);
                hashCode.Add(settings.DownloadClients.Lidarr.ApiKey ?? "");
            }

            return hashCode.ToHashCode();
        }
    }
} 