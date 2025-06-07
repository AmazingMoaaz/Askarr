using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Askarr.WebApi.config;
using Askarr.WebApi.AskarrBot.ChatClients.Telegram;
using Telegram.Bot;

namespace Askarr.WebApi.Controllers.ChatClients
{
    [ApiController]
    [Authorize]
    [Route("/api/chatclients")]
    public class ChatClientsController : ControllerBase
    {
        private readonly ChatClientsSettings _chatClientsSettings;
        private readonly BotClientSettings _botClientsSettings;
        private readonly Askarr.WebApi.AskarrBot.ChatClients.Telegram.TelegramSettings _telegramSettings;
        private readonly Askarr.WebApi.AskarrBot.ChatBot _chatBot;

        public ChatClientsController(
            ChatClientsSettingsProvider chatClientsSettingsProvider,
            BotClientSettingsProvider botClientSettingsProvider,
            TelegramSettingsProvider telegramSettingsProvider,
            Askarr.WebApi.AskarrBot.ChatBot chatBot)
        {
            _chatClientsSettings = chatClientsSettingsProvider?.Provide() ?? new ChatClientsSettings();
            _botClientsSettings = botClientSettingsProvider?.Provide() ?? new BotClientSettings();
            _chatBot = chatBot;
            
            // Add defensive coding - create a default TelegramSettings if provider is null
            if (telegramSettingsProvider == null)
            {
                _telegramSettings = new Askarr.WebApi.AskarrBot.ChatClients.Telegram.TelegramSettings
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
            else
            {
                try
                {
                    _telegramSettings = telegramSettingsProvider.Provide();
                }
                catch (Exception)
                {
                    // If any exception occurs during Provide(), use a default instance
                    _telegramSettings = new Askarr.WebApi.AskarrBot.ChatClients.Telegram.TelegramSettings
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
            }
        }

        [HttpGet()]
        public async Task<IActionResult> GetAsync()
        {
            // Determine the active clients from the settings
            string activeClient = _botClientsSettings.Client ?? "Discord";
            bool usesBothClients = activeClient.Contains(",");

            return Ok(new ChatClientsSettingsModel
            {
                Client = activeClient,
                // Discord settings
                StatusMessage = _chatClientsSettings.Discord.StatusMessage,
                BotToken = activeClient.Contains("Discord") ? _chatClientsSettings.Discord.BotToken : "",
                ClientId = activeClient.Contains("Discord") ? _chatClientsSettings.Discord.ClientId : "",
                EnableRequestsThroughDirectMessages = _chatClientsSettings.Discord.EnableRequestsThroughDirectMessages,
                TvShowRoles = _chatClientsSettings.Discord.TvShowRoles ?? Array.Empty<string>(),
                MovieRoles = _chatClientsSettings.Discord.MovieRoles ?? Array.Empty<string>(),
                MusicRoles = _chatClientsSettings.Discord.MusicRoles ?? Array.Empty<string>(),
                MonitoredChannels = _chatClientsSettings.Discord.MonitoredChannels ?? Array.Empty<string>(),
                AutomaticallyNotifyRequesters = _chatClientsSettings.Discord.AutomaticallyNotifyRequesters,
                NotificationMode = _chatClientsSettings.Discord.NotificationMode,
                NotificationChannels = _chatClientsSettings.Discord.NotificationChannels ?? Array.Empty<string>(),
                AutomaticallyPurgeCommandMessages = _chatClientsSettings.Discord.AutomaticallyPurgeCommandMessages,
                UsePrivateResponses = _chatClientsSettings.Discord.UsePrivateResponses,
                // Telegram settings
                TelegramBotToken = activeClient.Contains("Telegram") ? _telegramSettings.BotToken : "",
                TelegramMonitoredChats = _telegramSettings.MonitoredChats ?? Array.Empty<string>(),
                TelegramNotificationChats = _telegramSettings.NotificationChats ?? Array.Empty<string>(),
                TelegramMovieRoles = _telegramSettings.MovieRoles ?? Array.Empty<string>(),
                TelegramTvRoles = _telegramSettings.TvShowRoles ?? Array.Empty<string>(),
                TelegramMusicRoles = _telegramSettings.MusicRoles ?? Array.Empty<string>(),
                TelegramEnableRequestsThroughDirectMessages = _telegramSettings.EnableRequestsThroughDirectMessages,
                TelegramAutomaticallyNotifyRequesters = _telegramSettings.AutomaticallyNotifyRequesters,
                TelegramNotificationMode = _telegramSettings.NotificationMode ?? "PrivateMessages",
                // Common settings
                Language = !string.IsNullOrWhiteSpace(_chatClientsSettings.Language) 
                    ? System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_chatClientsSettings.Language.ToLower()) 
                    : "English",
                AvailableLanguages = GetLanguages()
            });
        }


        [HttpPost("discord/test")]
        public async Task<IActionResult> TestDiscordSettings([FromBody] ChatClientTestSettingsModel model)
        {
            try
            {
                var discord = new DiscordClient(new DiscordConfiguration
                {
                    Token = model.BotToken,
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.All,
                    AutoReconnect = false
                });

                var tcs = new TaskCompletionSource<string>();

                async Task ClientErrored(DiscordClient client, ClientErrorEventArgs args)
                {
                    tcs.TrySetResult("The bot is missing [Presence Intent] and/or [Server Members Intent] and/or [Message Content Intent], you must enable them in the bot discord settings and then restart the bot, check the wiki if you are unsure how to do this");
                }

                async Task SocketErrored(DiscordClient client, SocketErrorEventArgs args)
                {
                    tcs.TrySetResult("The bot is missing [Presence Intent] and/or [Server Members Intent] and/or [Message Content Intent], you must enable them in the bot discord settings and then restart the bot, check the wiki if you are unsure how to do this");
                }

                async Task Connected(DiscordClient client, ReadyEventArgs args)
                {
                    tcs.TrySetResult(string.Empty);
                }

                async Task Closed(DiscordClient client, SocketCloseEventArgs args)
                {
                    tcs.TrySetResult("The bot is missing [Presence Intent] and/or [Server Members Intent] and/or [Message Content Intent], you must enable them in the bot discord settings and then restart the bot, check the wiki if you are unsure how to do this");
                }

                discord.ClientErrored += ClientErrored;
                discord.Ready += Connected;
                discord.SocketErrored += SocketErrored;
                discord.SocketClosed += Closed;

                await discord.ConnectAsync();

                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(15));
                    tcs.TrySetException(new Exception());
                });

                var message = await tcs.Task;

                await discord.DisconnectAsync();
                discord.Dispose();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    return BadRequest(message);
                }

                return Ok(new { ok = true });
            }
            catch (System.Exception)
            {
                return BadRequest($"Invalid bot token");
            }
        }

        [HttpPost("telegram/test")]
        public async Task<IActionResult> TestTelegramSettings([FromBody] ChatClientTestSettingsModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.BotToken))
                {
                    return BadRequest(new { ok = false, error = "Bot token is required" });
                }
                
                // Create Telegram client with the provided token
                var botClient = new TelegramBotClient(model.BotToken);
                
                // Get information about the bot to verify the token works
                var me = await botClient.GetMeAsync();
                
                if (me != null && !string.IsNullOrEmpty(me.Username))
                {
                    return Ok(new { ok = true, botUsername = me.Username });
                }
                
                return BadRequest(new { ok = false, error = "Unable to connect to Telegram with the provided token" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ok = false, error = $"Invalid Telegram bot token: {ex.Message}" });
            }
        }

        [HttpPost()]
        public async Task<IActionResult> SaveAsync([FromBody] ChatClientsSettingsModel model)
        {
            try
            {
                // Validate client selection
                if (string.IsNullOrWhiteSpace(model.Client))
                {
                    return BadRequest("Please select a chat client");
                }

                bool useDiscord = model.Client.Contains("Discord", StringComparison.OrdinalIgnoreCase);
                bool useTelegram = model.Client.Contains("Telegram", StringComparison.OrdinalIgnoreCase);

                if (useDiscord)
                {
                    if (model.TvShowRoles.Any(x => !ulong.TryParse(x, System.Globalization.NumberStyles.Integer, null, out _)))
                    {
                        return BadRequest("Invalid tv show roles, please make sure to enter the discord role ids.");
                    }

                    if (model.MovieRoles.Any(x => !ulong.TryParse(x, System.Globalization.NumberStyles.Integer, null, out _)))
                    {
                        return BadRequest("Invalid movie roles, please make sure to enter the discord role ids.");
                    }

                    if (model.MusicRoles.Any(x => !ulong.TryParse(x, System.Globalization.NumberStyles.Integer, null, out _)))
                    {
                        return BadRequest("Invalid music roles, please make sure to enter the discord role ids.");
                    }

                    if (model.NotificationChannels.Any(x => !ulong.TryParse(x, System.Globalization.NumberStyles.Integer, null, out _)))
                    {
                        return BadRequest("Invalid notification channels, please make sure to enter the discord channel ids.");
                    }

                    if (model.MonitoredChannels.Any(x => !ulong.TryParse(x, System.Globalization.NumberStyles.Integer, null, out _)))
                    {
                        return BadRequest("Invalid monitored channels channels, please make sure to enter the monitored channel ids.");
                    }

                    // Update Discord settings
                    _chatClientsSettings.Discord.BotToken = model.BotToken?.Trim() ?? "";
                    _chatClientsSettings.Discord.ClientId = model.ClientId ?? "";
                    _chatClientsSettings.Discord.StatusMessage = model.StatusMessage?.Trim() ?? "";
                    _chatClientsSettings.Discord.TvShowRoles = (model.TvShowRoles ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                    _chatClientsSettings.Discord.MovieRoles = (model.MovieRoles ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                    _chatClientsSettings.Discord.MusicRoles = (model.MusicRoles ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                    _chatClientsSettings.Discord.EnableRequestsThroughDirectMessages = model.EnableRequestsThroughDirectMessages;
                    _chatClientsSettings.Discord.MonitoredChannels = (model.MonitoredChannels ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                    _chatClientsSettings.Discord.AutomaticallyNotifyRequesters = model.AutomaticallyNotifyRequesters;
                    _chatClientsSettings.Discord.NotificationMode = model.NotificationMode;
                    _chatClientsSettings.Discord.NotificationChannels = (model.NotificationChannels ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                    _chatClientsSettings.Discord.AutomaticallyPurgeCommandMessages = model.AutomaticallyPurgeCommandMessages;
                    _chatClientsSettings.Discord.UsePrivateResponses = model.UsePrivateResponses;
                }

                if (useTelegram)
                {
                    // Update Telegram settings
                    _telegramSettings.BotToken = model.TelegramBotToken?.Trim() ?? "";
                    _telegramSettings.EnableRequestsThroughDirectMessages = model.TelegramEnableRequestsThroughDirectMessages;
                    _telegramSettings.MonitoredChats = (model.TelegramMonitoredChats ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                    _telegramSettings.NotificationChats = (model.TelegramNotificationChats ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                    _telegramSettings.MovieRoles = (model.TelegramMovieRoles ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                    _telegramSettings.TvShowRoles = (model.TelegramTvRoles ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                    _telegramSettings.MusicRoles = (model.TelegramMusicRoles ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                    _telegramSettings.AutomaticallyNotifyRequesters = model.TelegramAutomaticallyNotifyRequesters;
                    _telegramSettings.NotificationMode = model.TelegramNotificationMode ?? "PrivateMessages";
                }

                // Update common settings
                if (string.IsNullOrWhiteSpace(model.Language))
                {
                    model.Language = "english"; // Default to English if no language provided
                }
                
                // Validate that the language file exists
                string languageFilePath = Program.CombindPath($"locales/{model.Language.ToLower()}.json");
                if (!System.IO.File.Exists(languageFilePath))
                {
                    return BadRequest($"The selected language '{model.Language}' is not available.");
                }
                
                _chatClientsSettings.Language = model.Language.ToLower();
                _botClientsSettings.Client = model.Client;

                // Save settings
                ChatClientsSettingsRepository.Update(_botClientsSettings, _chatClientsSettings);
                TelegramSettingsProvider.Update(_telegramSettings);
                
                // Refresh the Telegram bot if it's being used
                if (useTelegram)
                {
                    _chatBot?.Start();
                }

                return Ok(new { ok = true });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error saving settings: {ex.Message}");
            }
        }

        private string[] GetLanguages()
        {
            return Directory.GetFiles(Program.CombindPath("locales"), "*.json", SearchOption.TopDirectoryOnly)
                 .Select(x => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Path.GetFileNameWithoutExtension(x).ToLower()))
                 .ToArray();
        }
    }
}
