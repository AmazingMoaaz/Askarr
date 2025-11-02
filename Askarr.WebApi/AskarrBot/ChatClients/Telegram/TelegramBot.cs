using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Askarr.WebApi.AskarrBot.Movies;
using Askarr.WebApi.AskarrBot.TvShows;
using Askarr.WebApi.AskarrBot.Music;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Askarr.WebApi.AskarrBot.ChatClients.Telegram
{
    public class TelegramBot
    {
        private TelegramBotClient _botClient;
        private readonly ILogger<TelegramBot> _logger;
        private readonly TelegramSettingsProvider _telegramSettingsProvider;
        private readonly MovieWorkflowFactory _movieWorkflowFactory;
        private readonly TvShowWorkflowFactory _tvShowWorkflowFactory;
        private readonly MusicWorkflowFactory _musicWorkflowFactory;
        private TelegramSettings _currentSettings = new TelegramSettings();
        private CancellationTokenSource _cts;

        public TelegramBot(
            ILogger<TelegramBot> logger,
            TelegramSettingsProvider telegramSettingsProvider,
            MovieWorkflowFactory movieWorkflowFactory,
            TvShowWorkflowFactory tvShowWorkflowFactory,
            MusicWorkflowFactory musicWorkflowFactory)
        {
            _logger = logger;
            _telegramSettingsProvider = telegramSettingsProvider;
            _movieWorkflowFactory = movieWorkflowFactory;
            _tvShowWorkflowFactory = tvShowWorkflowFactory;
            _musicWorkflowFactory = musicWorkflowFactory;
        }

        public async Task Start()
        {
            try
            {
                _currentSettings = _telegramSettingsProvider.Provide();
                
                if (string.IsNullOrWhiteSpace(_currentSettings.BotToken))
                {
                    _logger.LogWarning("No Telegram bot token provided, skipping Telegram bot initialization");
                    return;
                }

                _botClient = new TelegramBotClient(_currentSettings.BotToken);
                _cts = new CancellationTokenSource();

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
                };

                _botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: _cts.Token
                );

                var me = await _botClient.GetMeAsync();
                _logger.LogInformation("Telegram bot started successfully: @{BotUsername}", me.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting Telegram bot");
            }
        }

        public async Task Stop()
        {
            _cts?.Cancel();
            _logger.LogInformation("Telegram bot stopped");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                // Handle callback queries (button clicks)
                if (update.CallbackQuery != null)
                {
                    await HandleCallbackQuery(update.CallbackQuery, cancellationToken);
                    return;
                }
                
                // Only process message updates
                if (update.Message is not { } message)
                    return;
                
                // Only process text messages
                if (message.Text is not { } messageText)
                    return;

                var chatId = message.Chat.Id;
                
                // Check if this chat is monitored (if not in DM mode)
                if (!_currentSettings.EnableRequestsThroughDirectMessages && 
                    !_currentSettings.MonitoredChats.Contains(chatId.ToString()))
                {
                    return;
                }

                _logger.LogInformation("Received a message in chat {ChatId}: {MessageText}", chatId, messageText);

                // Process commands (e.g., /help, /movie, /tv, /music)
                if (messageText.StartsWith("/"))
                {
                    await ProcessCommand(message, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling Telegram update");
            }
        }

        private async Task HandleCallbackQuery(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            try
            {
                var data = callbackQuery.Data;
                if (string.IsNullOrEmpty(data))
                    return;

                // Handle help menu callbacks
                if (data.StartsWith("help_"))
                {
                    await HandleHelpCallback(callbackQuery, data, cancellationToken);
                    return;
                }

                // Format is "action:parameter"
                var parts = data.Split(':', 2);
                if (parts.Length != 2)
                    return;

                var action = parts[0];
                var parameter = parts[1];
                
                // Acknowledge the callback query first to show loading state
                await _botClient.AnswerCallbackQueryAsync(
                    callbackQueryId: callbackQuery.Id,
                    text: "Processing your request...",
                    cancellationToken: cancellationToken);
                
                switch (action)
                {
                    case "movie_info":
                        await ShowMovieInfo(callbackQuery.Message.Chat.Id, parameter, cancellationToken);
                        break;
                    
                    case "movie_request":
                        await RequestMovie(callbackQuery.Message.Chat.Id, callbackQuery.From.Id, parameter, cancellationToken);
                        break;
                    
                    case "movie_watch":
                        await WatchMovie(callbackQuery.Message.Chat.Id, parameter, cancellationToken);
                        break;
                        
                    case "movie_more":
                        await ShowMoreMovieInfo(callbackQuery.Message.Chat.Id, parameter, cancellationToken);
                        break;
                        
                    case "movie_issue":
                        await ReportMovieIssue(callbackQuery.Message.Chat.Id, callbackQuery.From.Id, parameter, cancellationToken);
                        break;
                    
                    default:
                        await _botClient.SendTextMessageAsync(
                            chatId: callbackQuery.Message.Chat.Id,
                            text: "Unknown action. Please try again.",
                            cancellationToken: cancellationToken);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling callback query");
                try
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "❌ Sorry, there was an error processing your request. Please try again later.",
                        cancellationToken: cancellationToken);
                }
                catch { /* Suppress any errors in the error handler */ }
            }
        }

        private async Task ShowMovieInfo(long chatId, string movieId, CancellationToken cancellationToken)
        {
            try
            {
                // Create movie requester and interface
                var telegramUserInterface = new TelegramMovieUserInterface(_botClient, chatId, cancellationToken);
                var userRequester = new MovieUserRequester("system", "system");
                var request = new MovieRequest(userRequester, 0);
                
                // Get the movie details from the workflow factory
                var movie = await GetMovieById(movieId);
                if (movie == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "❌ Movie not found. It may have been removed from the database.",
                        cancellationToken: cancellationToken);
                    return;
                }
                
                // Show detailed movie info
                await telegramUserInterface.DisplayMovieDetailsAsync(request, movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing movie info: {MovieId}", movieId);
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "❌ Failed to get movie details. Please try again later.",
                    cancellationToken: cancellationToken);
            }
        }

        private async Task<Movie> GetMovieById(string movieId)
        {
            // This needs to match how the Discord bot gets movies by ID
            try
            {
                // Create a user requester (as the system)
                var userRequester = new MovieUserRequester("system", "system");
                var request = new MovieRequest(userRequester, 0);
                
                // Parse the movie ID to integer (TheMovieDbId is stored as int in IMovieSearcher)
                if (!int.TryParse(movieId, out int theMovieDbId))
                {
                    _logger.LogError("Invalid movie ID format: {MovieId}", movieId);
                    return null;
                }
                
                // Create a requesting workflow which has the method to get movies by ID
                var requestingWorkflow = new MovieRequestingWorkflow(
                    userRequester,
                    0,
                    // Get the searcher from the MovieWorkflowFactory
                    _movieWorkflowFactory.GetMovieSearcher(), 
                    null, // We don't need a requester for just getting the movie
                    new TelegramMovieUserInterface(_botClient, 0, CancellationToken.None), 
                    null  // We don't need notifications for just getting the movie
                );
                
                // Use the IMovieSearcher directly
                return await _movieWorkflowFactory.GetMovieSearcher().SearchMovieAsync(request, theMovieDbId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie by ID: {MovieId}", movieId);
                return null;
            }
        }

        private async Task RequestMovie(long chatId, long userId, string movieId, CancellationToken cancellationToken)
        {
            try
            {
                // Get the movie by ID
                var movie = await GetMovieById(movieId);
                if (movie == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "❌ Movie not found. It may have been removed from the database.",
                        cancellationToken: cancellationToken);
                    return;
                }
                
                // If movie is already available, send a different message
                if (movie.Available)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"✅ Good news! '{movie.Title}' is already available to watch!",
                        cancellationToken: cancellationToken);
                    
                    // Create buttons to watch the movie
                    var inlineKeyboard = new List<List<InlineKeyboardButton>>();
                    var watchButtons = new List<InlineKeyboardButton>();
                    
                    if (!string.IsNullOrEmpty(movie.PlexUrl))
                    {
                        watchButtons.Add(InlineKeyboardButton.WithUrl("▶️ Watch on Plex", movie.PlexUrl));
                    }
                    
                    if (!string.IsNullOrEmpty(movie.EmbyUrl))
                    {
                        watchButtons.Add(InlineKeyboardButton.WithUrl("▶️ Watch on Emby", movie.EmbyUrl));
                    }
                    
                    if (watchButtons.Count > 0)
                    {
                        inlineKeyboard.Add(watchButtons);
                        
                        await _botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Choose where to watch:",
                            replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
                            cancellationToken: cancellationToken);
                    }
                    
                    return;
                }
                
                // Send initial message that we're processing the request
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"🎬 Processing request for '{movie.Title}'...",
                    cancellationToken: cancellationToken);
                
                try
                {
                    // Create user requester and other necessary components
                    var userRequester = new MovieUserRequester(userId.ToString(), userId.ToString());
                    
                    // Parse the movie ID to integer
                    if (!int.TryParse(movieId, out int theMovieDbId))
                    {
                        _logger.LogError("Invalid movie ID format: {MovieId}", movieId);
                        await _botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "❌ Invalid movie ID format.",
                            cancellationToken: cancellationToken);
                        return;
                    }
                    
                    // Create a proper requesting workflow with all needed components
                    var requestingWorkflow = new MovieRequestingWorkflow(
                        userRequester,
                        0, // Use default category ID
                        _movieWorkflowFactory.GetMovieSearcher(),
                        _movieWorkflowFactory.GetMovieRequester(),
                        new TelegramMovieUserInterface(_botClient, chatId, cancellationToken),
                        _movieWorkflowFactory.CreateNotificationWorkflowForTelegram(userRequester)
                    );
                    
                    // Make the actual request through the workflow
                    await requestingWorkflow.RequestMovieAsync(theMovieDbId);
                    
                    // Success message will be sent by the user interface
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error requesting movie: {MovieId}", movieId);
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"❌ Failed to request '{movie.Title}'. Please try again later.",
                        cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing movie request: {MovieId}", movieId);
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "❌ Sorry, there was an error processing your movie request.",
                    cancellationToken: cancellationToken);
            }
        }

        private async Task WatchMovie(long chatId, string movieId, CancellationToken cancellationToken)
        {
            try
            {
                // Get the movie by ID
                var movie = await GetMovieById(movieId);
                if (movie == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "❌ Movie not found. It may have been removed from the database.",
                        cancellationToken: cancellationToken);
                    return;
                }
                
                // If the movie is not available, handle that case
                if (!movie.Available)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"⚠️ '{movie.Title}' is not available to watch yet. Would you like to request it?",
                        cancellationToken: cancellationToken);
                    
                    // Create a request button
                    var inlineKeyboard = new List<List<InlineKeyboardButton>>
                    {
                        new List<InlineKeyboardButton>
                        {
                            InlineKeyboardButton.WithCallbackData("🔔 Request Movie", $"movie_request:{movie.TheMovieDbId}")
                        }
                    };
                    
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Request this movie?",
                        replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
                        cancellationToken: cancellationToken);
                    
                    return;
                }
                
                // Create buttons to watch the movie on different platforms
                var watchButtons = new List<InlineKeyboardButton>();
                
                if (!string.IsNullOrEmpty(movie.PlexUrl))
                {
                    watchButtons.Add(InlineKeyboardButton.WithUrl("▶️ Watch on Plex", movie.PlexUrl));
                }
                
                if (!string.IsNullOrEmpty(movie.EmbyUrl))
                {
                    watchButtons.Add(InlineKeyboardButton.WithUrl("▶️ Watch on Emby", movie.EmbyUrl));
                }
                
                if (!string.IsNullOrEmpty(movie.MediaUrl))
                {
                    watchButtons.Add(InlineKeyboardButton.WithUrl("▶️ Watch", movie.MediaUrl));
                }
                
                if (watchButtons.Count > 0)
                {
                    var inlineKeyboard = new List<List<InlineKeyboardButton>> { watchButtons };
                    
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"🎬 '{movie.Title}' is ready to watch! Choose your platform:",
                        replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"✅ '{movie.Title}' is available, but no direct watch links are configured.",
                        cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing watch request: {MovieId}", movieId);
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "❌ Sorry, there was an error processing your watch request.",
                    cancellationToken: cancellationToken);
            }
        }

        private async Task ShowMoreMovieInfo(long chatId, string movieId, CancellationToken cancellationToken)
        {
            try
            {
                // Get the movie by ID
                var movie = await GetMovieById(movieId);
                if (movie == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "❌ Movie not found. It may have been removed from the database.",
                        cancellationToken: cancellationToken);
                    return;
                }
                
                // Create an expanded movie information message
                var year = !string.IsNullOrEmpty(movie.ReleaseDate) ? $" ({movie.ReleaseDate.Substring(0, 4)})" : "";
                var message = $"🎬 *{movie.Title}*{year}\n\n";
                
                if (!string.IsNullOrEmpty(movie.Overview))
                {
                    message += $"📝 *Overview*: {movie.Overview}\n\n";
                }
                
                message += $"🆔 *ID*: {movie.TheMovieDbId}\n";
                
                if (!string.IsNullOrEmpty(movie.Quality))
                {
                    message += $"🔍 *Quality*: {movie.Quality}\n";
                }
                
                message += $"📊 *Status*: {(movie.Available ? "✅ Available" : "⏳ Not Available")}\n";
                message += $"🔍 *Requested*: {(movie.Requested ? "Yes" : "No")}\n";
                
                // Create action buttons
                var inlineKeyboard = new List<List<InlineKeyboardButton>>();
                var row1 = new List<InlineKeyboardButton>();
                
                if (movie.Available)
                {
                    row1.Add(InlineKeyboardButton.WithCallbackData("▶️ Watch Now", $"movie_watch:{movie.TheMovieDbId}"));
                }
                else if (!movie.Requested)
                {
                    row1.Add(InlineKeyboardButton.WithCallbackData("🔔 Request Movie", $"movie_request:{movie.TheMovieDbId}"));
                }
                
                // Add the first row of buttons if any
                if (row1.Count > 0)
                {
                    inlineKeyboard.Add(row1);
                }
                
                // Add a button to report issues
                inlineKeyboard.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("⚙️ Report Issue", $"movie_issue:{movie.TheMovieDbId}")
                });
                
                // Send the expanded info message with buttons
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: message,
                    parseMode: ParseMode.Markdown,
                    replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing more movie info: {MovieId}", movieId);
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "❌ Sorry, there was an error retrieving additional movie information.",
                    cancellationToken: cancellationToken);
            }
        }

        private async Task ReportMovieIssue(long chatId, long userId, string movieId, CancellationToken cancellationToken)
        {
            try
            {
                // Get the movie by ID
                var movie = await GetMovieById(movieId);
                if (movie == null)
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "❌ Movie not found. It may have been removed from the database.",
                        cancellationToken: cancellationToken);
                    return;
                }
                
                // Define common issue types
                var issueTypes = new Dictionary<string, string>
                {
                    { "audio_issue", "Audio Issues" },
                    { "subtitle_issue", "Subtitle Issues" },
                    { "video_quality", "Video Quality Issues" },
                    { "wrong_movie", "Wrong Movie" },
                    { "playback_error", "Playback Error" },
                    { "other_issue", "Other Issue" }
                };
                
                // Create an inline keyboard with issue options
                var inlineKeyboard = new List<List<InlineKeyboardButton>>();
                
                foreach (var issue in issueTypes)
                {
                    inlineKeyboard.Add(new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{issue.Value}",
                            $"report_issue:{movie.TheMovieDbId}:{issue.Key}")
                    });
                }
                
                // Send a message with issue options
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"🛠️ Report an issue with '{movie.Title}':\nPlease select the type of issue you're experiencing:",
                    replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up movie issue reporting: {MovieId}", movieId);
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "❌ Sorry, there was an error setting up issue reporting.",
                    cancellationToken: cancellationToken);
            }
        }

        private async Task ProcessCommand(Message message, CancellationToken cancellationToken)
        {
            var commandParts = message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = commandParts[0].ToLowerInvariant();
            
            // Extract mention part if present (for commands like /ping@BotName)
            if (command.Contains('@'))
            {
                command = command.Split('@')[0];
            }
            
            switch (command)
            {
                case "/help":
                    await SendHelpMessage(message.Chat.Id, cancellationToken);
                    break;
                
                case "/ping":
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Pong! Bot is online and responding.",
                        cancellationToken: cancellationToken);
                    break;
                
                case "/movie":
                    if (_currentSettings.MovieDownloadClient != "Disabled")
                    {
                        if (commandParts.Length > 1)
                        {
                            var searchQuery = string.Join(" ", commandParts.Skip(1));
                            await HandleMovieRequest(message.Chat.Id, message.From.Id, searchQuery, cancellationToken);
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Please provide a movie title. Example: /movie The Matrix",
                                cancellationToken: cancellationToken);
                        }
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Movie requests are disabled.",
                            cancellationToken: cancellationToken);
                    }
                    break;
                
                case "/tv":
                    if (_currentSettings.TvShowDownloadClient != "Disabled")
                    {
                        if (commandParts.Length > 1)
                        {
                            var searchQuery = string.Join(" ", commandParts.Skip(1));
                            await HandleTvShowRequest(message.Chat.Id, message.From.Id, searchQuery, cancellationToken);
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Please provide a TV show title. Example: /tv Breaking Bad",
                                cancellationToken: cancellationToken);
                        }
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "TV show requests are disabled.",
                            cancellationToken: cancellationToken);
                    }
                    break;
                
                case "/music":
                    if (_currentSettings.MusicDownloadClient != "Disabled")
                    {
                        if (commandParts.Length > 1)
                        {
                            var searchQuery = string.Join(" ", commandParts.Skip(1));
                            await HandleMusicRequest(message.Chat.Id, message.From.Id, searchQuery, cancellationToken);
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Please provide an artist or album name. Example: /music Pink Floyd",
                                cancellationToken: cancellationToken);
                        }
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Music requests are disabled.",
                            cancellationToken: cancellationToken);
                    }
                    break;
                
                default:
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Unknown command. Type /help for available commands.",
                        cancellationToken: cancellationToken);
                    break;
            }
        }

        private async Task HandleHelpCallback(CallbackQuery callbackQuery, string data, CancellationToken cancellationToken)
        {
            try
            {
                // Acknowledge the callback first
                await _botClient.AnswerCallbackQueryAsync(
                    callbackQueryId: callbackQuery.Id,
                    cancellationToken: cancellationToken);

                string messageText = "";
                
                switch (data)
                {
                    case "help_movie":
                        messageText = "🎬 <b>Request a Movie</b>\n\n" +
                                     "Please send the movie title you want to request:\n\n" +
                                     "📝 <b>Format:</b> <code>/movie [movie title]</code>\n\n" +
                                     "💡 <b>Examples:</b>\n" +
                                     "• <code>/movie The Matrix</code>\n" +
                                     "• <code>/movie Inception</code>\n" +
                                     "• <code>/movie Avatar 2</code>\n\n" +
                                     "Just type your command below! ⬇️";
                        break;
                    case "help_tv":
                        messageText = "📺 <b>Request a TV Show</b>\n\n" +
                                     "Please send the TV show title you want to request:\n\n" +
                                     "📝 <b>Format:</b> <code>/tv [show title]</code>\n\n" +
                                     "💡 <b>Examples:</b>\n" +
                                     "• <code>/tv Breaking Bad</code>\n" +
                                     "• <code>/tv Game of Thrones</code>\n" +
                                     "• <code>/tv The Last of Us</code>\n\n" +
                                     "Just type your command below! ⬇️";
                        break;
                    case "help_music":
                        messageText = "🎵 <b>Request Music</b>\n\n" +
                                     "Please send the artist or album name:\n\n" +
                                     "📝 <b>Format:</b> <code>/music [artist/album]</code>\n\n" +
                                     "💡 <b>Examples:</b>\n" +
                                     "• <code>/music Pink Floyd</code>\n" +
                                     "• <code>/music The Beatles</code>\n" +
                                     "• <code>/music Taylor Swift</code>\n\n" +
                                     "Just type your command below! ⬇️";
                        break;
                    case "help_ping":
                        messageText = "✅ <b>Bot Status: Online</b>\n\n" +
                                     "The bot is running and ready to process your requests!\n\n" +
                                     "Type /help to see all available commands.";
                        break;
                    default:
                        messageText = "❌ Unknown help topic.";
                        break;
                }
                
                // Send a new message with instructions
                await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: messageText,
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling help callback");
            }
        }

        private async Task SendHelpMessage(long chatId, CancellationToken cancellationToken)
        {
            var helpMessage = "🤖 <b>Askarr Media Bot Commands</b>\n\n" +
                              "Available commands:\n\n";
            
            var commandsList = new List<string>();
            commandsList.Add("🔹 <b>/help</b> - Show this help message");
            commandsList.Add("🔹 <b>/ping</b> - Check if the bot is online");
            
            if (_currentSettings.MovieDownloadClient != "Disabled")
            {
                commandsList.Add("🔹 <b>/movie</b> &lt;title&gt; - Search and request a movie");
            }
            
            if (_currentSettings.TvShowDownloadClient != "Disabled")
            {
                commandsList.Add("🔹 <b>/tv</b> &lt;title&gt; - Search and request a TV show");
            }
            
            if (_currentSettings.MusicDownloadClient != "Disabled")
            {
                commandsList.Add("🔹 <b>/music</b> &lt;artist/album&gt; - Search and request music");
            }

            helpMessage += string.Join("\n\n", commandsList);
            helpMessage += "\n\n━━━━━━━━━━━━━━━━━━━\n";
            helpMessage += "💡 <b>Quick Actions:</b>\n";
            helpMessage += "Click a button below to get started!";
            
            // Create inline keyboard with quick command examples
            var inlineKeyboard = new List<List<InlineKeyboardButton>>();
            
            if (_currentSettings.MovieDownloadClient != "Disabled")
            {
                inlineKeyboard.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("🎬 Request Movie", "help_movie")
                });
            }
            
            if (_currentSettings.TvShowDownloadClient != "Disabled")
            {
                inlineKeyboard.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("📺 Request TV Show", "help_tv")
                });
            }
            
            if (_currentSettings.MusicDownloadClient != "Disabled")
            {
                inlineKeyboard.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("🎵 Request Music", "help_music")
                });
            }
            
            // Add status check button
            inlineKeyboard.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("✅ Check Bot Status", "help_ping")
            });
            
            var replyMarkup = new InlineKeyboardMarkup(inlineKeyboard);
            
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: helpMessage,
                parseMode: ParseMode.Html,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
        }

        private async Task HandleMovieRequest(long chatId, long userId, string searchQuery, CancellationToken cancellationToken)
        {
            try
            {
                // Send dynamic initial message
                var searchMessages = new[]
                {
                    $"🔍 Searching for \"{searchQuery}\"...",
                    $"🎬 Looking up \"{searchQuery}\" in the database...",
                    $"🎞️ Finding matches for \"{searchQuery}\"...",
                    $"🍿 Searching for movie: \"{searchQuery}\"..."
                };
                
                var random = new Random();
                var searchMessage = searchMessages[random.Next(searchMessages.Length)];
                
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: searchMessage,
                    cancellationToken: cancellationToken);
                
                // Create a user interface adapter for Telegram
                var telegramUserInterface = new TelegramMovieUserInterface(_botClient, chatId, cancellationToken);
                
                // Create a movie requester object
                var userRequester = new MovieUserRequester(userId.ToString(), userId.ToString());
                
                // Create a workflow just for searching
                var searchWorkflow = new MovieSearchWorkflow(userRequester, _movieWorkflowFactory, telegramUserInterface, _logger);
                
                // Perform the search
                var movies = await searchWorkflow.SearchMoviesAsync(searchQuery);
                
                if (movies == null || !movies.Any())
                {
                    var notFoundMessages = new[]
                    {
                        $"😕 No movies found matching '{searchQuery}'",
                        $"🔍 Couldn't find any movies matching '{searchQuery}'",
                        $"📭 No results for '{searchQuery}'. Try a different search term?"
                    };
                    
                    await _botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: notFoundMessages[random.Next(notFoundMessages.Length)],
                        cancellationToken: cancellationToken);
                    return;
                }
                
                // Format results with more context
                var resultMessage = movies.Count == 1 
                    ? $"Found 1 movie matching your search:\n\n"
                    : $"Found {Math.Min(movies.Count, 5)} of {movies.Count} movies matching your search:\n\n";
                
                // Create inline keyboard for interactive movie selection
                var inlineKeyboard = new List<List<InlineKeyboardButton>>();
                
                foreach (var movie in movies.Take(5))
                {
                    var year = !string.IsNullOrEmpty(movie.ReleaseDate) ? $" ({movie.ReleaseDate.Substring(0, 4)})" : "";
                    var status = movie.Available ? "✅ Available" : "⏳ Unavailable";
                    resultMessage += $"• {movie.Title}{year} - {status}\n";
                    
                    // Add row for each movie with two buttons: Info and Request
                    var row = new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"ℹ️ Info: {movie.Title.Substring(0, Math.Min(movie.Title.Length, 20))}{(movie.Title.Length > 20 ? "..." : "")}",
                            $"movie_info:{movie.TheMovieDbId}"),
                        
                        InlineKeyboardButton.WithCallbackData(
                            movie.Available ? "▶️ Watch" : "🔔 Request",
                            $"movie_request:{movie.TheMovieDbId}")
                    };
                    
                    inlineKeyboard.Add(row);
                }
                
                resultMessage += "\n📌 Select a movie below to see details or make a request:";
                
                // Send results with interactive buttons
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: resultMessage,
                    replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for movie: {Query}", searchQuery);
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "😓 Sorry, there was an error processing your movie request. Please try again later.",
                    cancellationToken: cancellationToken);
            }
        }

        private async Task HandleTvShowRequest(long chatId, long userId, string searchQuery, CancellationToken cancellationToken)
        {
            // Simplified implementation - in a real scenario, you would integrate with the actual TV show workflow
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Searching for TV show: {searchQuery}...",
                cancellationToken: cancellationToken);
            
            // TODO: Implement actual TV show request workflow
        }

        private async Task HandleMusicRequest(long chatId, long userId, string searchQuery, CancellationToken cancellationToken)
        {
            // Simplified implementation - in a real scenario, you would integrate with the actual music workflow
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Searching for music: {searchQuery}...",
                cancellationToken: cancellationToken);
            
            // TODO: Implement actual music request workflow
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogError("Telegram polling error: {ErrorMessage}", errorMessage);
            return Task.CompletedTask;
        }

        public async Task RefreshSettings()
        {
            try
            {
                var newSettings = _telegramSettingsProvider.Provide();
                
                // If token changed or was newly added, restart the bot
                if (_currentSettings.BotToken != newSettings.BotToken)
                {
                    await Stop();
                    _currentSettings = newSettings;
                    await Start();
                }
                else
                {
                    _currentSettings = newSettings;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing Telegram bot settings");
            }
        }
    }

    public class TelegramMovieUserInterface : IMovieUserInterface
    {
        private readonly TelegramBotClient _botClient;
        private readonly long _chatId;
        private readonly CancellationToken _cancellationToken;

        public TelegramMovieUserInterface(TelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            _botClient = botClient;
            _chatId = chatId;
            _cancellationToken = cancellationToken;
        }

        public async Task WarnNoMovieFoundAsync(string movieName)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"No movies found matching '{movieName}'",
                cancellationToken: _cancellationToken);
        }

        public async Task ShowMovieSelection(MovieRequest request, IReadOnlyList<Movie> movies)
        {
            // Create a message with movie details
            var resultMessage = $"Found {Math.Min(movies.Count, 5)} of {movies.Count} matching movies:\n\n";
            
            // Create inline keyboard for interactive movie selection
            var inlineKeyboard = new List<List<InlineKeyboardButton>>();
            
            foreach (var movie in movies.Take(5))
            {
                var year = !string.IsNullOrEmpty(movie.ReleaseDate) ? $" ({movie.ReleaseDate.Substring(0, 4)})" : "";
                var availableIcon = movie.Available ? "✅" : "⏳";
                resultMessage += $"{availableIcon} {movie.Title}{year}\n";
                
                if (!string.IsNullOrEmpty(movie.Overview) && movie.Overview.Length > 100)
                {
                    resultMessage += $"   {movie.Overview.Substring(0, 97)}...\n";
                }
                else if (!string.IsNullOrEmpty(movie.Overview))
                {
                    resultMessage += $"   {movie.Overview}\n";
                }
                resultMessage += "\n";
                
                // Add row for each movie with two buttons: Info and Request
                var row = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(
                        $"ℹ️ Info: {movie.Title.Substring(0, Math.Min(movie.Title.Length, 20))}{(movie.Title.Length > 20 ? "..." : "")}",
                        $"movie_info:{movie.TheMovieDbId}"),
                    
                    InlineKeyboardButton.WithCallbackData(
                        movie.Available ? "▶️ Watch" : "🔔 Request",
                        $"movie_request:{movie.TheMovieDbId}")
                };
                
                inlineKeyboard.Add(row);
            }
            
            resultMessage += "\n📌 Select a movie below to see details or make a request:";
            
            // Send message with interactive keyboard
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: resultMessage,
                replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
                cancellationToken: _cancellationToken);
        }

        public async Task ConfirmSelectedMovieAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Selected movie: {movie.Title}",
                cancellationToken: _cancellationToken);
        }

        public async Task WarnAlreadyRequestedAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Movie '{movie.Title}' has already been requested.",
                cancellationToken: _cancellationToken);
        }

        public async Task WarnUnavailableMovieAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Movie '{movie.Title}' is currently unavailable.",
                cancellationToken: _cancellationToken);
        }

        public async Task WarnNoMovieFoundByTheMovieDbIdAsync(string theMovieDbId)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"No movie found with ID {theMovieDbId}.",
                cancellationToken: _cancellationToken);
        }

        public async Task ConfirmRequestedMovieAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Successfully requested '{movie.Title}'.",
                cancellationToken: _cancellationToken);
        }

        public async Task ShowIssueSelectionAsync(MovieRequest request, IReadOnlyDictionary<string, int> issueTypes)
        {
            var message = "Please select an issue type:\n\n";
            foreach (var issue in issueTypes)
            {
                message += $"• {issue.Key}\n";
            }
            
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: message,
                cancellationToken: _cancellationToken);
        }

        public async Task ShowMovieIssueSelection(MovieRequest request, IReadOnlyList<Movie> movies)
        {
            var message = "Please select a movie to report an issue:\n\n";
            foreach (var movie in movies.Take(5))
            {
                var year = !string.IsNullOrEmpty(movie.ReleaseDate) ? $" ({movie.ReleaseDate.Substring(0, 4)})" : "";
                message += $"• {movie.Title}{year}\n";
            }
            
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: message,
                cancellationToken: _cancellationToken);
        }

        public async Task ConfirmSelectedIssueAsync(string issue)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Selected issue: {issue}",
                cancellationToken: _cancellationToken);
        }

        public async Task WarnMovieAlreadyAvailableAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Movie '{movie.Title}' is already available.",
                cancellationToken: _cancellationToken);
        }

        public async Task DisplayMovieDetailsAsync(MovieRequest request, Movie movie)
        {
            var year = !string.IsNullOrEmpty(movie.ReleaseDate) ? $" ({movie.ReleaseDate.Substring(0, 4)})" : "";
            var status = movie.Available ? "✅ Available" : "⏳ Not Available";
            
            var message = $"🎬 **Movie Details**\n\n" +
                          $"🎞️ **Title**: {movie.Title}{year}\n" +
                          $"📊 **Status**: {status}\n";
            
            if (!string.IsNullOrEmpty(movie.Overview))
            {
                message += $"📝 **Overview**: {movie.Overview}\n";
            }
            
            if (!string.IsNullOrEmpty(movie.Quality))
            {
                message += $"🔍 **Quality**: {movie.Quality}\n";
            }
            
            if (movie.Available)
            {
                if (!string.IsNullOrEmpty(movie.PlexUrl))
                {
                    message += $"🎬 **Watch on Plex**: {movie.PlexUrl}\n";
                }
                
                if (!string.IsNullOrEmpty(movie.EmbyUrl))
                {
                    message += $"🎬 **Watch on Emby**: {movie.EmbyUrl}\n";
                }
            }
            
            // Create action buttons for the movie
            var inlineKeyboard = new List<List<InlineKeyboardButton>>();
            
            // First row of buttons
            var row1 = new List<InlineKeyboardButton>();
            
            if (movie.Available)
            {
                // If the movie is available, add a Watch button
                row1.Add(InlineKeyboardButton.WithCallbackData("▶️ Watch Now", $"movie_watch:{movie.TheMovieDbId}"));
            }
            else
            {
                // If not available, add a Request button
                row1.Add(InlineKeyboardButton.WithCallbackData("🔔 Request Movie", $"movie_request:{movie.TheMovieDbId}"));
            }
            
            // Add buttons for the first row
            inlineKeyboard.Add(row1);
            
            // Second row of buttons
            var row2 = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("📥 More Info", $"movie_more:{movie.TheMovieDbId}"),
                InlineKeyboardButton.WithCallbackData("⚙️ Report Issue", $"movie_issue:{movie.TheMovieDbId}")
            };
            
            // Add buttons for the second row
            inlineKeyboard.Add(row2);
            
            // Send the message with the inline keyboard
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: message,
                replyMarkup: new InlineKeyboardMarkup(inlineKeyboard),
                cancellationToken: _cancellationToken);
        }

        public async Task DisplayMovieIssueDetailsAsync(MovieRequest request, Movie movie, string issue)
        {
            var message = $"Issue Report:\n\n" +
                          $"Movie: {movie.Title}\n" +
                          $"Issue: {issue}";
            
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: message,
                cancellationToken: _cancellationToken);
        }

        public async Task DisplayMovieIssueModalAsync(MovieRequest request, Movie movie, string issue)
        {
            await DisplayMovieIssueDetailsAsync(request, movie, issue);
        }

        public async Task CompleteMovieIssueModalRequestAsync(Movie movie, bool success)
        {
            var message = success 
                ? $"Issue for '{movie.Title}' has been successfully reported." 
                : $"Failed to report issue for '{movie.Title}'.";
            
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: message,
                cancellationToken: _cancellationToken);
        }

        public async Task DisplayRequestDeniedAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Request for '{movie.Title}' was denied.",
                cancellationToken: _cancellationToken);
        }

        public async Task DisplayRequestSuccessAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Request for '{movie.Title}' was successful!",
                cancellationToken: _cancellationToken);
        }

        public async Task WarnMovieUnavailableAndAlreadyHasNotificationAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Movie '{movie.Title}' is unavailable, but you're already on the notification list.",
                cancellationToken: _cancellationToken);
        }

        public async Task WarnMovieAlreadyRequestedAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Movie '{movie.Title}' has already been requested.",
                cancellationToken: _cancellationToken);
        }

        public async Task DisplayNotificationSuccessAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"You will be notified when '{movie.Title}' becomes available.",
                cancellationToken: _cancellationToken);
        }

        public async Task AskForNotificationRequestAsync(Movie movie)
        {
            await _botClient.SendTextMessageAsync(
                chatId: _chatId,
                text: $"Movie '{movie.Title}' is currently unavailable. Would you like to be notified when it becomes available?",
                cancellationToken: _cancellationToken);
        }
    }

    public class MovieSearchWorkflow
    {
        private readonly MovieUserRequester _user;
        private readonly MovieWorkflowFactory _movieWorkflowFactory;
        private readonly IMovieUserInterface _userInterface;
        private readonly ILogger _logger;

        public MovieSearchWorkflow(
            MovieUserRequester user,
            MovieWorkflowFactory movieWorkflowFactory,
            IMovieUserInterface userInterface,
            ILogger logger)
        {
            _user = user;
            _movieWorkflowFactory = movieWorkflowFactory;
            _userInterface = userInterface;
            _logger = logger;
        }

        public async Task<IReadOnlyList<Movie>> SearchMoviesAsync(string movieName)
        {
            try
            {
                // Create a movie request
                var request = new MovieRequest(_user, 0);
                
                try
                {
                    // Use the direct search method in the workflow factory
                    return await _movieWorkflowFactory.SearchDirectAsync(request, movieName);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error using direct search for movie: {MovieName}", movieName);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error searching for movie: {MovieName}", movieName);
                await _userInterface.WarnNoMovieFoundAsync(movieName);
                return Array.Empty<Movie>();
            }
        }
    }
} 