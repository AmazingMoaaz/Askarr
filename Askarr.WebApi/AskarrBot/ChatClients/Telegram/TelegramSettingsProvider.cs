using System;
using System.Linq;
using Askarr.WebApi.AskarrBot.DownloadClients;
using Askarr.WebApi.AskarrBot.DownloadClients.Lidarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Ombi;
using Askarr.WebApi.AskarrBot.DownloadClients.Overseerr;
using Askarr.WebApi.AskarrBot.DownloadClients.Radarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Sonarr;
using Askarr.WebApi.AskarrBot.Extensions;

namespace Askarr.WebApi.AskarrBot.ChatClients.Telegram
{
    public class TelegramSettingsProvider
    {
        public TelegramSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new TelegramSettings
            {
                BotToken = settings.ChatClients.Telegram.BotToken,
                Username = settings.ChatClients.Telegram.Username,
                MovieDownloadClient = settings.Movies.Client,
                MovieDownloadClientConfigurationHash = ComputeMovieClientConfigurationHashCode(settings),
                TvShowDownloadClient = settings.TvShows.Client,
                TvShowDownloadClientConfigurationHash = ComputeTvClientConfigurationHashCode(settings),
                MusicDownloadClient = settings.Music.Client,
                MusicDownloadClientConfigurationHash = ComputeMusicClientConfigurationHashCode(settings),
                MonitoredChats = settings.ChatClients.Telegram.MonitoredChats.ToObject<string[]>(),
                TvShowRoles = settings.ChatClients.Telegram.TvShowRoles.ToObject<string[]>(),
                MovieRoles = settings.ChatClients.Telegram.MovieRoles.ToObject<string[]>(),
                MusicRoles = settings.ChatClients.Telegram.MusicRoles.ToObject<string[]>(),
                EnableRequestsThroughDirectMessages = settings.ChatClients.Telegram.EnableRequestsThroughDirectMessages,
                AutomaticallyNotifyRequesters = settings.ChatClients.Telegram.AutomaticallyNotifyRequesters,
                NotificationMode = settings.ChatClients.Telegram.NotificationMode,
                NotificationChats = settings.ChatClients.Telegram.NotificationChats.ToObject<string[]>(),
            };
        }

        private int ComputeMovieClientConfigurationHashCode(dynamic settings)
        {
            var hashCode = new HashCode();

            if (settings.Movies.Client == "Radarr")
            {
                hashCode.Add(settings.DownloadClients.Radarr.Hostname);
                hashCode.Add(settings.DownloadClients.Radarr.Port);
                hashCode.Add(settings.DownloadClients.Radarr.ApiKey);
            }
            else if (settings.Movies.Client == "Ombi")
            {
                hashCode.Add(settings.DownloadClients.Ombi.Hostname);
                hashCode.Add(settings.DownloadClients.Ombi.Port);
                hashCode.Add(settings.DownloadClients.Ombi.ApiKey);
                hashCode.Add(settings.DownloadClients.Ombi.ApiUsername);
            }
            else if (settings.Movies.Client == "Overseerr")
            {
                hashCode.Add(settings.DownloadClients.Overseerr.Hostname);
                hashCode.Add(settings.DownloadClients.Overseerr.Port);
                hashCode.Add(settings.DownloadClients.Overseerr.ApiKey);
                hashCode.Add(settings.DownloadClients.Overseerr.Movies.DefaultApiUserID);
            }

            return hashCode.ToHashCode();
        }

        private int ComputeTvClientConfigurationHashCode(dynamic settings)
        {
            var hashCode = new HashCode();

            if (settings.TvShows.Client == "Sonarr")
            {
                hashCode.Add(settings.DownloadClients.Sonarr.Hostname);
                hashCode.Add(settings.DownloadClients.Sonarr.Port);
                hashCode.Add(settings.DownloadClients.Sonarr.ApiKey);
            }
            else if (settings.TvShows.Client == "Ombi")
            {
                hashCode.Add(settings.DownloadClients.Ombi.Hostname);
                hashCode.Add(settings.DownloadClients.Ombi.Port);
                hashCode.Add(settings.DownloadClients.Ombi.ApiKey);
                hashCode.Add(settings.DownloadClients.Ombi.ApiUsername);
            }
            else if (settings.TvShows.Client == "Overseerr")
            {
                hashCode.Add(settings.DownloadClients.Overseerr.Hostname);
                hashCode.Add(settings.DownloadClients.Overseerr.Port);
                hashCode.Add(settings.DownloadClients.Overseerr.ApiKey);
                hashCode.Add(settings.DownloadClients.Overseerr.TvShows.DefaultApiUserID);
            }

            return hashCode.ToHashCode();
        }

        private int ComputeMusicClientConfigurationHashCode(dynamic settings)
        {
            var hashCode = new HashCode();

            if (settings.Music.Client == "Lidarr")
            {
                hashCode.Add(settings.DownloadClients.Lidarr.Hostname);
                hashCode.Add(settings.DownloadClients.Lidarr.Port);
                hashCode.Add(settings.DownloadClients.Lidarr.ApiKey);
            }

            return hashCode.ToHashCode();
        }
    }
} 