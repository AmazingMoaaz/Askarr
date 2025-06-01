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

        private async Task ProcessCommand(Message message, CancellationToken cancellationToken)
        {
            var commandParts = message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = commandParts[0].ToLowerInvariant();
            
            switch (command)
            {
                case "/help":
                    await SendHelpMessage(message.Chat.Id, cancellationToken);
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

        private async Task SendHelpMessage(long chatId, CancellationToken cancellationToken)
        {
            var helpMessage = "Available commands:\n" +
                              "/help - Show this help message\n";
            
            if (_currentSettings.MovieDownloadClient != "Disabled")
            {
                helpMessage += "/movie [title] - Request a movie\n";
            }
            
            if (_currentSettings.TvShowDownloadClient != "Disabled")
            {
                helpMessage += "/tv [title] - Request a TV show\n";
            }
            
            if (_currentSettings.MusicDownloadClient != "Disabled")
            {
                helpMessage += "/music [artist/album] - Request music\n";
            }
            
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: helpMessage,
                cancellationToken: cancellationToken);
        }

        private async Task HandleMovieRequest(long chatId, long userId, string searchQuery, CancellationToken cancellationToken)
        {
            // Simplified implementation - in a real scenario, you would integrate with the actual movie workflow
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Searching for movie: {searchQuery}...",
                cancellationToken: cancellationToken);
            
            // TODO: Implement actual movie request workflow
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
} 