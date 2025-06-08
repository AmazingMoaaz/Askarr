using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Askarr.WebApi.Extensions;
using Askarr.WebApi.AskarrBot.ChatClients.Discord;
using Askarr.WebApi.AskarrBot.ChatClients.Telegram;
using Askarr.WebApi.AskarrBot.DownloadClients;
using Askarr.WebApi.AskarrBot.DownloadClients.Lidarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Ombi;
using Askarr.WebApi.AskarrBot.DownloadClients.Overseerr;
using Askarr.WebApi.AskarrBot.DownloadClients.Radarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Sonarr;
using Askarr.WebApi.AskarrBot.Locale;
using Askarr.WebApi.AskarrBot.Movies;
using Askarr.WebApi.AskarrBot.Music;
using Askarr.WebApi.AskarrBot.Notifications;
using Askarr.WebApi.AskarrBot.Notifications.Movies;
using Askarr.WebApi.AskarrBot.Notifications.Music;
using Askarr.WebApi.AskarrBot.Notifications.TvShows;
using Askarr.WebApi.AskarrBot.TvShows;

namespace Askarr.WebApi.AskarrBot
{
    public class ChatBot
    {
        private DiscordClient _client;
        private TelegramBot _telegramBot;
        private MovieNotificationEngine _movieNotificationEngine;
        private TvShowNotificationEngine _tvShowNotificationEngine;
        private MusicNotificationEngine _musicNotificationEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ChatBot> _logger;
        private readonly DiscordSettingsProvider _discordSettingsProvider;
        private readonly TelegramSettingsProvider _telegramSettingsProvider;
        private readonly ConcurrentBag<Func<Task>> _refreshQueue = new ConcurrentBag<Func<Task>>();
        private DiscordSettings _currentSettings = new DiscordSettings();
        private MovieWorkflowFactory _movieWorkflowFactory;
        private TvShowWorkflowFactory _tvShowWorkflowFactory;
        private MusicWorkflowFactory _musicWorkflowFactory;
        private MovieNotificationsRepository _movieNotificationRepository = new MovieNotificationsRepository();
        private TvShowNotificationsRepository _tvShowNotificationRepository = new TvShowNotificationsRepository();
        private MusicNotificationsRepository _musicNotificationRepository = new MusicNotificationsRepository();
        private OverseerrClient _overseerrClient;
        private OmbiClient _ombiDownloadClient;
        private RadarrClient _radarrDownloadClient;
        private SonarrClient _sonarrDownloadClient;
        private LidarrClient _lidarrDownloadClient;
        private SlashCommandsExtension _slashCommands = null;
        private HashSet<ulong> _currentGuilds = new HashSet<ulong>();
        private Language _previousLanguage = Language.Current;
        private int _waitTimeout = 0;

        public ChatBot(
            IServiceProvider serviceProvider,
            ILogger<ChatBot> logger,
            DiscordSettingsProvider discordSettingsProvider,
            TelegramSettingsProvider telegramSettingsProvider,
            MovieWorkflowFactory movieWorkflowFactory,
            TvShowWorkflowFactory tvShowWorkflowFactory,
            MusicWorkflowFactory musicWorkflowFactory,
            OverseerrClient overseerrClient,
            OmbiClient ombiDownloadClient,
            RadarrClient radarrDownloadClient,
            SonarrClient sonarrDownloadClient,
            LidarrClient lidarrDownloadClient)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _discordSettingsProvider = discordSettingsProvider;
            _telegramSettingsProvider = telegramSettingsProvider;
            _movieWorkflowFactory = movieWorkflowFactory;
            _tvShowWorkflowFactory = tvShowWorkflowFactory;
            _musicWorkflowFactory = musicWorkflowFactory;
            _overseerrClient = overseerrClient;
            _ombiDownloadClient = ombiDownloadClient;
            _radarrDownloadClient = radarrDownloadClient;
            _sonarrDownloadClient = sonarrDownloadClient;
            _lidarrDownloadClient = lidarrDownloadClient;
        }

        public void Start()
        {
            try
            {
                _movieWorkflowFactory = _serviceProvider.Get<MovieWorkflowFactory>();
                _tvShowWorkflowFactory = _serviceProvider.Get<TvShowWorkflowFactory>();
                _musicWorkflowFactory = _serviceProvider.Get<MusicWorkflowFactory>();
                _overseerrClient = _serviceProvider.Get<OverseerrClient>();
                _ombiDownloadClient = _serviceProvider.Get<OmbiClient>();
                _radarrDownloadClient = _serviceProvider.Get<RadarrClient>();
                _sonarrDownloadClient = _serviceProvider.Get<SonarrClient>();
                _lidarrDownloadClient = _serviceProvider.Get<LidarrClient>();

                BotClientSettings settings = _serviceProvider.Get<BotClientSettingsProvider>().Provide();
                var botClient = settings?.Client ?? string.Empty;

                // Create temporary directory if it doesn't exist
                if (!Directory.Exists(SlashCommandBuilder.TempFolder))
                {
                    Console.WriteLine("No temp folder found, creating one...");
                    Directory.CreateDirectory(SlashCommandBuilder.TempFolder);
                }

                // Start Telegram bot if configured, regardless of Discord status
                if (botClient == "Telegram" || botClient == "Both")
                {
                    try
                    {
                        _logger.LogInformation("Starting Telegram bot...");
                        StartTelegramBot();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to start Telegram chat client");
                    }
                }

                // Start Discord bot if configured
                if (botClient == "Discord" || botClient == "Both")
                {
                    try 
                    {
                        _logger.LogInformation("Starting Discord bot...");
                        StartDiscordBot();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to start Discord chat client");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start chat bot");
            }
        }

        private void StartDiscordBot()
        {
            try
            {
                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        if (_waitTimeout > 0)
                            _waitTimeout--;
                        try
                        {
                            bool guildsChanged = false;
                            var newSettings = _discordSettingsProvider.Provide();

                            try
                            {
                                HashSet<ulong> guilds = new HashSet<ulong>(_client?.Guilds.Keys.ToArray() ?? Array.Empty<ulong>());
                                guildsChanged = !(new HashSet<ulong>(guilds).SetEquals(_currentGuilds)) && _client != null;

                                if (guildsChanged)
                                {
                                    _currentGuilds.Clear();
                                    _currentGuilds.UnionWith(guilds);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                _logger.LogError(ex, "Error checking guild changes: " + ex.Message);
                                guildsChanged = false;
                            }

                            // Force client initialization if not present
                            if (_client == null && _waitTimeout <= 0 && !string.IsNullOrWhiteSpace(newSettings.BotToken))
                            {
                                _logger.LogInformation("Initializing Discord client for the first time");
                                _currentSettings = newSettings;
                                _previousLanguage = Language.Current;
                                await RestartBot(new DiscordSettings(), newSettings, new HashSet<ulong>());
                            }
                            else if (!_currentSettings.Equals(newSettings) || Language.Current != _previousLanguage || guildsChanged || (_client == null && _waitTimeout <= 0 && !string.IsNullOrWhiteSpace(newSettings.BotToken)))
                            {
                                var previousSettings = _currentSettings;
                                _logger.LogWarning("Bot configuration changed: restarting bot");
                                _currentSettings = newSettings;
                                _previousLanguage = Language.Current;
                                await RestartBot(previousSettings, newSettings, _currentGuilds);
                                _logger.LogWarning("Bot has been restarted.");

                                SlashCommandBuilder.CleanUp();

                                //Delay till next restart
                                if(_waitTimeout <= 0)
                                    _waitTimeout = 5;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error while restarting the bot: " + ex.Message);
                        }

                        await Task.Delay(5000);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start Discord chat client");
            }
        }

        private void StartTelegramBot()
        {
            try
            {
                var telegramLogger = _serviceProvider.GetService<ILogger<TelegramBot>>();
                _telegramBot = new TelegramBot(
                    telegramLogger,
                    _telegramSettingsProvider,
                    _movieWorkflowFactory,
                    _tvShowWorkflowFactory,
                    _musicWorkflowFactory);
                
                Task.Run(async () => await _telegramBot.Start());
                
                _logger.LogInformation("Telegram bot starting...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start Telegram chat client");
            }
        }

        private async Task DisposeClient()
        {
            if (_client != null)
            {
                await _client.DisconnectAsync();
                _client.Ready -= Connected;
                _client.ComponentInteractionCreated -= DiscordComponentInteractionCreatedHandler;
                _client.ModalSubmitted -= DiscordModalSubmittedHandler;
                _client.Dispose();
            }

            if (_slashCommands != null)
            {
                _slashCommands.SlashCommandErrored -= SlashCommandErrorHandler;
                _slashCommands.Dispose();
            }
        }

        private async Task RestartBot(DiscordSettings previousSettings, DiscordSettings newSettings, HashSet<ulong> currentGuilds)
        {
            if (!string.IsNullOrEmpty(newSettings.BotToken))
            {
                if (!string.Equals(previousSettings.BotToken, newSettings.BotToken, StringComparison.OrdinalIgnoreCase) || _client == null || _slashCommands == null)
                {
                    _logger.LogInformation("Discord bot token changed or client not initialized - creating new client");
                    await DisposeClient();

                    var config = new DiscordConfiguration()
                    {
                        Token = newSettings.BotToken,
                        TokenType = TokenType.Bot,
                        AutoReconnect = true,
                        MinimumLogLevel = LogLevel.Warning,
                        Intents = DiscordIntents.All,
                        ReconnectIndefinitely = true
                    };

                    _client = new DiscordClient(config);

                    _slashCommands = _client.UseSlashCommands(new SlashCommandsConfiguration
                    {
                        Services = new ServiceCollection()
                            .AddSingleton<DiscordClient>(_client)
                            .AddSingleton<ILogger>(_logger)
                            .AddSingleton<DiscordSettingsProvider>(_discordSettingsProvider)
                            .AddSingleton<MovieWorkflowFactory>(_movieWorkflowFactory)
                            .AddSingleton<TvShowWorkflowFactory>(_tvShowWorkflowFactory)
                            .AddSingleton<MusicWorkflowFactory>(_musicWorkflowFactory)
                            .BuildServiceProvider()
                    });

                    _slashCommands.SlashCommandErrored += SlashCommandErrorHandler;

                    _client.Ready += Connected;
                    _client.ComponentInteractionCreated += DiscordComponentInteractionCreatedHandler;
                    _client.ModalSubmitted += DiscordModalSubmittedHandler;

                    _currentGuilds = new HashSet<ulong>();

                    try
                    {
                        _logger.LogInformation("Connecting to Discord...");
                        await _client.ConnectAsync();
                        _logger.LogInformation("Successfully connected to Discord");
                    }
                    catch (Exception ex) when (ex.InnerException is DSharpPlus.Exceptions.UnauthorizedException)
                    {
                        _logger.LogError("Discord token is incorrect, please check your token settings.");
                        _client = null;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error connecting to Discord, error: {ex.Message}");
                        _client = null;
                    }
                }

                if (_client != null)
                {
                    if (_client.Guilds.Any())
                    {
                        try
                        {
                            _logger.LogInformation("Applying bot configuration...");
                            await ApplyBotConfigurationAsync(newSettings);
                            _logger.LogInformation("Bot configuration applied successfully");

                            var prop = _slashCommands.GetType().GetProperty("_updateList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            prop.SetValue(_slashCommands, new List<KeyValuePair<ulong?, Type>>());

                            _logger.LogInformation("Building slash commands...");
                            var slashCommandType = SlashCommandBuilder.Build(_logger, newSettings, _serviceProvider.Get<RadarrSettingsProvider>(), _serviceProvider.Get<SonarrSettingsProvider>(), _serviceProvider.Get<OverseerrSettingsProvider>(), _serviceProvider.Get<OmbiSettingsProvider>(), _serviceProvider.Get<LidarrSettingsProvider>());

                            if (newSettings.EnableRequestsThroughDirectMessages)
                            {
                                _logger.LogInformation("Registering global slash commands...");
                                try { _slashCommands.RegisterCommands(slashCommandType); }
                                catch (System.Exception ex) { _logger.LogError(ex, "Error while registering global slash commands: " + ex.Message); }

                                foreach (var guildId in _client.Guilds.Keys)
                                {
                                    try { _slashCommands.RegisterCommands<EmptySlashCommands>(guildId); }
                                    catch (System.Exception ex) { _logger.LogError(ex, $"Error while emptying guild-specific slash commands for guid {guildId}: " + ex.Message); }
                                }
                            }
                            else
                            {
                                _logger.LogInformation("Registering guild-specific slash commands...");
                                try { _slashCommands.RegisterCommands<EmptySlashCommands>(); }
                                catch (System.Exception ex) { _logger.LogError(ex, "Error while emptying global slash commands: " + ex.Message); }

                                foreach (var guildId in _client.Guilds.Keys)
                                {
                                    try { _slashCommands.RegisterCommands(slashCommandType, guildId); }
                                    catch (System.Exception ex) { _logger.LogError(ex, $"Error while registering guild-specific slash commands for guid {guildId}: " + ex.Message); }
                                }
                            }

                            _logger.LogInformation("Refreshing commands...");
                            await _slashCommands.RefreshCommands();
                            _logger.LogInformation("Discord bot ready!");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error setting up Discord bot\n{ex.Message}");

                            await DisposeClient();
                            _client = null;
                            _slashCommands = null;

                            //Wait for about 60 seconds till trying again
                            //Could be network issue
                            _waitTimeout = 12;
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No Bot Token for Discord has been configured.");
                }
            }
        }

        private async Task Connected(DiscordClient client, ReadyEventArgs args)
        {
            await ApplyBotConfigurationAsync(_currentSettings);
        }

        private async Task ApplyBotConfigurationAsync(DiscordSettings discordSettings)
        {
            try
            {
                await _client.UpdateStatusAsync(new DiscordActivity(discordSettings.StatusMessage, ActivityType.Playing));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error update the bot's status: " + ex.Message);
            }

            try
            {
                if (_movieNotificationEngine != null)
                {
                    await _movieNotificationEngine.StopAsync();
                }

                if (_currentSettings.MovieDownloadClient != DownloadClient.Disabled && _currentSettings.NotificationMode != NotificationMode.Disabled)
                {
                    _movieNotificationEngine = _movieWorkflowFactory.CreateMovieNotificationEngine(_client, _logger);
                }

                _movieNotificationEngine?.Start();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while starting movie notification engine: " + ex.Message);
            }

            try
            {
                if (_tvShowNotificationEngine != null)
                {
                    await _tvShowNotificationEngine.StopAsync();
                }

                if (_currentSettings.TvShowDownloadClient != DownloadClient.Disabled && _currentSettings.NotificationMode != NotificationMode.Disabled)
                {
                    _tvShowNotificationEngine = _tvShowWorkflowFactory.CreateTvShowNotificationEngine(_client, _logger);
                }

                _tvShowNotificationEngine?.Start();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while starting tv show notification engine: " + ex.Message);
            }


            try
            {
                if (_musicNotificationEngine != null)
                {
                    await _musicNotificationEngine.StopAsync();
                }

                if (_currentSettings.MusicDownloadClient != DownloadClient.Disabled && _currentSettings.NotificationMode != NotificationMode.Disabled)
                {
                    _musicNotificationEngine = _musicWorkflowFactory.CreateMusicNotificationEngine(_client, _logger);
                }

                _musicNotificationEngine?.Start();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while starting music notification engine: " + ex.Message);
            }
        }


        /// <summary>
        /// Handles any Modal inputs form the user
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task DiscordModalSubmittedHandler(DiscordClient client, ModalSubmitEventArgs e)
        {
            try
            {
                if (e.Values.FirstOrDefault().Key.ToLower().StartsWith("mir"))
                {
                    await HandleMovieIssueModalAsync(e);
                }
                else if (e.Values.FirstOrDefault().Key.ToLower().StartsWith("tir"))
                {
                    await HandleTvShowIssueModalAsync(e);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while handling interaction: " + ex.Message);
                await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(Language.Current.Error));
            }
        }



        private async Task DiscordComponentInteractionCreatedHandler(DiscordClient client, ComponentInteractionCreateEventArgs e)
        {
            try
            {
                if (!e.Id.ToLower().Contains("modal"))
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);

                var authorId = ulong.Parse(e.Id.Split("/").Skip(1).First());

                if (e.User.Id == authorId)
                {
                    if (e.Id.ToLower().StartsWith("mr"))
                    {
                        await HandleMovieRequestAsync(e);
                    }
                    else if (e.Id.ToLower().StartsWith("mir"))
                    {
                        await HandleMovieIssueAsync(e);
                    }
                    else if (e.Id.ToLower().StartsWith("mnr"))
                    {
                        await CreateMovieNotificationWorkflow(e)
                            .AddNotificationAsync(e.Id.Split("/").Skip(1).First(), int.Parse(e.Id.Split("/").Last()));
                    }
                    else if (e.Id.ToLower().StartsWith("tr") || e.Id.ToLower().StartsWith("ts"))
                    {
                        await HandleTvRequestAsync(e);
                    }
                    else if (e.Id.ToLower().StartsWith("tir"))
                    {
                        await HandleTvIssueRequestAsync(e);
                    }
                    else if (e.Id.ToLower().StartsWith("tnr"))
                    {
                        var splitValues = e.Id.Split("/").Skip(1).ToArray();
                        var userId = splitValues[0];
                        var tvDbId = int.Parse(splitValues[1]);
                        var seasonType = splitValues[2];
                        var seasonNumber = splitValues[3];

                        await CreateTvShowNotificationWorkflow(e)
                            .AddNotificationAsync(userId, tvDbId, seasonType, int.Parse(seasonNumber));
                    }
                    else if (e.Id.ToLower().StartsWith("mur"))
                    {
                        await HandleMusicRequestAsync(e);
                    }
                    else if (e.Id.ToLower().StartsWith("munr"))
                    {
                        await CreateMusicNotificationWorkflow(e)
                            .AddNotificationArtistAsync(e.Id.Split("/").Skip(1).First(), e.Id.Split("/").Last());
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while handling interaction: " + ex.Message);
                await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(Language.Current.Error));
            }
        }

        private async Task SlashCommandErrorHandler(SlashCommandsExtension extension, SlashCommandErrorEventArgs args)
        {
            try
            {
                var settings = _serviceProvider.Get<DiscordSettingsProvider>()?.Provide() ?? new DiscordSettings { UsePrivateResponses = false };
                
                if (args.Exception is SlashExecutionChecksFailedException slex)
                {
                    foreach (var check in slex.FailedChecks)
                        if (check is RequireChannelsAttribute requireChannelAttribute)
                            await args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(settings.UsePrivateResponses).WithContent(Language.Current.DiscordCommandNotAvailableInChannel));
                        else if (check is RequireRolesAttribute requireRoleAttribute)
                            await args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(settings.UsePrivateResponses).WithContent(Language.Current.DiscordCommandMissingRoles));
                        else
                            await args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(settings.UsePrivateResponses).WithContent(Language.Current.DiscordCommandUnknownPrecondition));
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error while handling interaction error: " + ex.Message);
            }
        }

        private async Task HandleMovieRequestAsync(ComponentInteractionCreateEventArgs e)
        {
            if (e.Id.ToLower().StartsWith("mrs"))
            {
                if (e.Values != null && e.Values.Any())
                {
                    await CreateMovieRequestWorkFlow(e, int.Parse(e.Values.Single().Split("/").First()))
                        .HandleMovieSelectionAsync(int.Parse(e.Values.Single().Split("/").Last()));
                }
            }
            else if (e.Id.ToLower().StartsWith("mrc"))
            {
                var categoryId = int.Parse(e.Id.Split("/").Skip(2).First());

                await CreateMovieRequestWorkFlow(e, categoryId)
                    .RequestMovieAsync(int.Parse(e.Id.Split("/").Last()));
            }
        }


        /// <summary>
        /// Handles the request back to the user for an issue
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task HandleMovieIssueAsync(ComponentInteractionCreateEventArgs e)
        {
            if (e.Id.ToLower().StartsWith("mirs"))
            {
                if (e.Values != null && e.Values.Any())
                {
                    string[] values = e.Values.Single().Split("/");
                    string category = values[0];
                    string movie = values[1];
                    string issue = values.Length >= 3 ? values[2] : string.Empty;

                    await CreateMovieIssueWorkFlow(e, int.Parse(category))
                        .HandleIssueMovieSelectionAsync(int.Parse(movie), issue);
                }
            }
            else if (e.Id.ToLower().StartsWith("mirb"))
            {
                //Pull out the details form the message link
                string[] values = e.Id.Split("/");
                string category = values[2];
                string movie = values[3];
                string issue = values[4];

                await CreateMovieIssueWorkFlow(e, int.Parse(category))
                    .HandleIssueMovieSendModalAsync(int.Parse(movie), issue);
            }
        }


        /// <summary>
        /// Handle the modal input form the user connected to issues
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task HandleMovieIssueModalAsync(ModalSubmitEventArgs e)
        {
            KeyValuePair<string, string> firstTextbox = e.Values.FirstOrDefault();

            if (firstTextbox.Key.ToLower().StartsWith("mirc"))
            {
                string[] values = firstTextbox.Key.Split("/");
                string category = values[2];

                await CreateMovieIssueWorkFlow(e, int.Parse(category))
                    .SubmitIssueMovieModalReadAsync(firstTextbox);
            }
        }


        private async Task HandleTvRequestAsync(ComponentInteractionCreateEventArgs e)
        {
            if (e.Id.ToLower().StartsWith("trs"))
            {
                if (e.Values != null && e.Values.Any())
                {
                    await CreateTvShowRequestWorkFlow(e, int.Parse(e.Values.Single().Split("/").First()))
                        .HandleTvShowSelectionAsync(int.Parse(e.Values.Single().Split("/").Last()));
                }
            }
            else if (e.Id.ToLower().StartsWith("tss"))
            {
                if (e.Values != null && e.Values.Any())
                {
                    var splitValues = e.Values.Single().Split("/");
                    var categoryId = int.Parse(splitValues[0]);
                    var tvDbId = int.Parse(splitValues[1]);
                    var seasonNumber = int.Parse(splitValues[2]);

                    await CreateTvShowRequestWorkFlow(e, categoryId)
                        .HandleSeasonSelectionAsync(tvDbId, seasonNumber);
                }
            }
            else if (e.Id.ToLower().StartsWith("trc"))
            {
                var splitValues = e.Id.Split("/").Skip(2).ToArray();
                var categoryId = int.Parse(splitValues[0]);
                var tvDbId = int.Parse(splitValues[1]);
                var seasonNumber = int.Parse(splitValues[2]);

                await CreateTvShowRequestWorkFlow(e, categoryId)
                    .RequestSeasonSelectionAsync(tvDbId, seasonNumber);
            }
        }


        /// <summary>
        /// Handles issue responces
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task HandleTvIssueRequestAsync(ComponentInteractionCreateEventArgs e)
        {
            if (e.Id.ToLower().StartsWith("tirs"))
            {
                if (e.Values != null && e.Values.Any())
                {
                    string[] values = e.Values.Single().Split("/");
                    string category = values[0];
                    string tvShow = values[1];
                    string issue = values.Length >= 3 ? values[2] : string.Empty;

                    //TODO: finish here
                    await CreateTvShowIssueWorkFlow(e, int.Parse(category)).HandleIssueTVSelectionAsync(int.Parse(tvShow), issue);

                    //.HandleIssueMovieSelectionAsync(int.Parse(movie), issue);
                }
            }
            else if (e.Id.ToLower().StartsWith("tirb"))
            {
                //Pull out the details form the message link
                string[] values = e.Id.Split("/");
                string category = values[2];
                string tvShow = values[3];
                string issue = values[4];

                await CreateTvShowIssueWorkFlow(e, int.Parse(category))
                    .HandleIssueTvShowSendModalAsync(int.Parse(tvShow), issue);
            }
        }


        /// <summary>
        /// Handle the modal input form the user connected to issues
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task HandleTvShowIssueModalAsync(ModalSubmitEventArgs e)
        {
            KeyValuePair<string, string> firstTextbox = e.Values.FirstOrDefault();

            if (firstTextbox.Key.ToLower().StartsWith("tirc"))
            {
                string[] values = firstTextbox.Key.Split("/");
                string category = values[2];

                await CreateTvShowIssueWorkFlow(e, int.Parse(category))
                    .SubmitIssueTvShowModalReadAsync(firstTextbox);
            }
        }



        /// <summary>
        /// Handles requests for music artist coming in
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task HandleMusicRequestAsync(ComponentInteractionCreateEventArgs e)
        {
            if (e.Id.ToLower().StartsWith("mursa"))
            {
                if (e.Values != null && e.Values.Any())
                {
                    await CreateMusicRequestWorkFlow(e, int.Parse(e.Values.Single().Split("/").First()))
                        .HandleMusicArtistSelectionAsync(e.Values.Single().Split("/").Last());
                }
            }
            else if (e.Id.ToLower().StartsWith("murca"))
            {
                var categoryId = int.Parse(e.Id.Split("/").Skip(2).First());

                await CreateMusicRequestWorkFlow(e, categoryId)
                    .RequestMusicArtistAsync(e.Id.Split("/").Last());
            }
        }



        /// <summary>
        /// Returns Music Requesting workflow
        /// </summary>
        /// <param name="e"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private MusicRequestingWorkflow CreateMusicRequestWorkFlow(ComponentInteractionCreateEventArgs e, int categoryId)
        {
            return _musicWorkflowFactory.CreateRequestingWorkflow(e.Interaction, categoryId);
        }



        private MovieRequestingWorkflow CreateMovieRequestWorkFlow(ComponentInteractionCreateEventArgs e, int categoryId)
        {
            return _movieWorkflowFactory
                .CreateRequestingWorkflow(e.Interaction, categoryId);
        }


        /// <summary>
        /// Used to connect the movie issue workflow
        /// </summary>
        /// <param name="e"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private MovieIssueWorkflow CreateMovieIssueWorkFlow(ComponentInteractionCreateEventArgs e, int categoryId)
        {
            return _movieWorkflowFactory
                .CreateIssueWorkflow(e.Interaction, categoryId);
        }


        private MovieIssueWorkflow CreateMovieIssueWorkFlow(ModalSubmitEventArgs e, int categoryId)
        {
            return _movieWorkflowFactory
                .CreateIssueWorkflow(e.Interaction, categoryId);
        }


        private IMovieNotificationWorkflow CreateMovieNotificationWorkflow(ComponentInteractionCreateEventArgs e)
        {
            return _movieWorkflowFactory
                .CreateNotificationWorkflow(e.Interaction);
        }

        private TvShowRequestingWorkflow CreateTvShowRequestWorkFlow(ComponentInteractionCreateEventArgs e, int categoryId)
        {
            return _tvShowWorkflowFactory
                .CreateRequestingWorkflow(e.Interaction, categoryId);
        }


        private IMusicNotificationWorkflow CreateMusicNotificationWorkflow(ComponentInteractionCreateEventArgs e)
        {
            return _musicWorkflowFactory
                .CreateNotificationWorkflow(e.Interaction);
        }


        /// <summary>
        /// Returns a Workflow for creating issues
        /// </summary>
        /// <param name="e"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private TvShowIssueWorkflow CreateTvShowIssueWorkFlow(ComponentInteractionCreateEventArgs e, int categoryId)
        {
            return _tvShowWorkflowFactory
                .CreateIssueWorkflow(e.Interaction, categoryId);
        }

        private TvShowIssueWorkflow CreateTvShowIssueWorkFlow(ModalSubmitEventArgs e, int categoryId)
        {
            return _tvShowWorkflowFactory
                .CreateIssueWorkflow(e.Interaction, categoryId);
        }


        private ITvShowNotificationWorkflow CreateTvShowNotificationWorkflow(ComponentInteractionCreateEventArgs e)
        {
            return _tvShowWorkflowFactory
                .CreateNotificationWorkflow(e.Interaction);
        }
    }
}