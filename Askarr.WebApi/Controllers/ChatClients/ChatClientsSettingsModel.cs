using System.ComponentModel.DataAnnotations;

namespace  Askarr.WebApi.Controllers.ChatClients
{
    public class ChatClientsSettingsModel
    {
        [Required]
        public string Client { get; set; }

        // Discord specific settings
        public string ClientId { get; set; }
        public string BotToken { get; set; }
        public string[] MonitoredChannels { get; set; }
        public string[] TvShowRoles { get; set; }
        public string[] MovieRoles { get; set; }
        public string[] MusicRoles { get; set; }
        public bool EnableRequestsThroughDirectMessages { get; set; }
        public string[] NotificationChannels { get; set; }
        public bool AutomaticallyPurgeCommandMessages { get; set; }
        public bool UsePrivateResponses { get; set; }

        // Telegram specific settings
        public string TelegramBotToken { get; set; }
        public string[] TelegramMonitoredChats { get; set; }
        public string[] TelegramNotificationChats { get; set; }
        public string[] TelegramMovieRoles { get; set; }
        public string[] TelegramTvRoles { get; set; }
        public string[] TelegramMusicRoles { get; set; }
        public bool TelegramEnableRequestsThroughDirectMessages { get; set; }
        public bool TelegramAutomaticallyNotifyRequesters { get; set; }
        public string TelegramNotificationMode { get; set; }

        // Common settings
        public string StatusMessage { get; set; }
        public bool AutomaticallyNotifyRequesters { get; set; }
        public string NotificationMode { get; set; }
        public string Language { get; set; }
        public string[] AvailableLanguages { get; set; }
    }

    public class ChatClientTestSettingsModel
    {
        [Required]
        public string BotToken { get; set; }
        public string ChatClient { get; set; }
    }
}
