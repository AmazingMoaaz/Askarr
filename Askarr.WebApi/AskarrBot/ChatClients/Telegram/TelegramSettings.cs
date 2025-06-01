using System;
using System.Linq;

namespace Askarr.WebApi.AskarrBot.ChatClients.Telegram
{
    public class TelegramSettings
    {
        public string BotToken { get; set; }
        public string Username { get; set; }
        public string[] TvShowRoles { get; set; }
        public string[] MovieRoles { get; set; }
        public string[] MusicRoles { get; set; }
        public string[] MonitoredChats { get; set; }
        public string MovieDownloadClient { get; set; }
        public int MovieDownloadClientConfigurationHash { get; set; }
        public string TvShowDownloadClient { get; set; }
        public int TvShowDownloadClientConfigurationHash { get; set; }
        public string MusicDownloadClient { get; set; }
        public int MusicDownloadClientConfigurationHash { get; set; }
        public bool EnableRequestsThroughDirectMessages { get; set; }
        public bool AutomaticallyNotifyRequesters { get; set; }
        public string NotificationMode { get; set; }
        public string[] NotificationChats { get; set; }
    }
} 