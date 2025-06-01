using Askarr.WebApi.config;
using Askarr.WebApi.AskarrBot;

namespace Askarr.WebApi.Controllers.ChatClients
{
    public class ChatClientsSettingsProvider
    {
        public ChatClientsSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            var discordSettings = new DiscordSettings
            {
                BotToken = (string)settings.ChatClients.Discord.BotToken,
                ClientId = (string)settings.ChatClients.Discord.ClientId,
                StatusMessage = (string)settings.ChatClients.Discord.StatusMessage,
                MonitoredChannels = settings.ChatClients.Discord.MonitoredChannels.ToObject<string[]>(),
                TvShowRoles = settings.ChatClients.Discord.TvShowRoles.ToObject<string[]>(),
                MovieRoles = settings.ChatClients.Discord.MovieRoles.ToObject<string[]>(),
                MusicRoles = settings.ChatClients.Discord.MusicRoles.ToObject<string[]>(),
                EnableRequestsThroughDirectMessages = (bool)settings.ChatClients.Discord.EnableRequestsThroughDirectMessages,
                AutomaticallyNotifyRequesters = (bool)settings.ChatClients.Discord.AutomaticallyNotifyRequesters,
                NotificationMode = (string)settings.ChatClients.Discord.NotificationMode,
                NotificationChannels = settings.ChatClients.Discord.NotificationChannels.ToObject<string[]>(),
                AutomaticallyPurgeCommandMessages = (bool)settings.ChatClients.Discord.AutomaticallyPurgeCommandMessages,
            };

            var telegramSettings = new TelegramSettings
            {
                BotToken = (string)settings.ChatClients.Telegram.BotToken,
                Username = (string)settings.ChatClients.Telegram.Username,
                MonitoredChats = settings.ChatClients.Telegram.MonitoredChats.ToObject<string[]>(),
                TvShowRoles = settings.ChatClients.Telegram.TvShowRoles.ToObject<string[]>(),
                MovieRoles = settings.ChatClients.Telegram.MovieRoles.ToObject<string[]>(),
                MusicRoles = settings.ChatClients.Telegram.MusicRoles.ToObject<string[]>(),
                EnableRequestsThroughDirectMessages = (bool)settings.ChatClients.Telegram.EnableRequestsThroughDirectMessages,
                AutomaticallyNotifyRequesters = (bool)settings.ChatClients.Telegram.AutomaticallyNotifyRequesters,
                NotificationMode = (string)settings.ChatClients.Telegram.NotificationMode,
                NotificationChats = settings.ChatClients.Telegram.NotificationChats.ToObject<string[]>(),
            };

            return new ChatClientsSettings
            {
                Discord = discordSettings,
                Telegram = telegramSettings,
                Language = settings.ChatClients.Language,
            };
        }
    }
}