using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Askarr.WebApi.AskarrBot.ChatClients.Discord;
using Askarr.WebApi.AskarrBot.DownloadClients;
using Askarr.WebApi.AskarrBot.DownloadClients.Ombi;
using Askarr.WebApi.AskarrBot.DownloadClients.Overseerr;
using Askarr.WebApi.AskarrBot.DownloadClients.Radarr;
using Askarr.WebApi.AskarrBot.Notifications;
using Askarr.WebApi.AskarrBot.Notifications.Movies;

namespace Askarr.WebApi.AskarrBot.Movies
{
    public class MovieWorkflowFactory
    {
        private readonly DiscordSettingsProvider _settingsProvider;
        private readonly MovieNotificationsRepository _notificationsRepository;
        private OverseerrClient _overseerrClient;
        private OmbiClient _ombiDownloadClient;
        private RadarrClient _radarrDownloadClient;

        public MovieWorkflowFactory(
            DiscordSettingsProvider settingsProvider,
            MovieNotificationsRepository notificationsRepository,
            OverseerrClient overseerrClient,
            OmbiClient ombiDownloadClient,
            RadarrClient radarrDownloadClient)
        {
            _settingsProvider = settingsProvider;
            _notificationsRepository = notificationsRepository;
            _overseerrClient = overseerrClient;
            _ombiDownloadClient = ombiDownloadClient;
            _radarrDownloadClient = radarrDownloadClient;
        }

        public MovieRequestingWorkflow CreateRequestingWorkflow(DiscordInteraction interaction, int categoryId)
        {
            var settings = _settingsProvider.Provide();

            return new MovieRequestingWorkflow(new MovieUserRequester(interaction.User.Id.ToString(),  interaction.User.Username),
                                                categoryId,
                                                GetMovieClient<IMovieSearcher>(settings),
                                                GetMovieClient<IMovieRequester>(settings),
                                                new DiscordMovieUserInterface(interaction, GetMovieClient<IMovieSearcher>(settings)),
                                                CreateMovieNotificationWorkflow(interaction, settings));
        }


        /// <summary>
        /// This handles creating a issue for a movie
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public MovieIssueWorkflow CreateIssueWorkflow(DiscordInteraction interaction, int categoryId)
        {
            var settings = _settingsProvider.Provide();

            return new MovieIssueWorkflow(new MovieUserRequester(interaction.User.Id.ToString(), interaction.User.Username),
                                                categoryId,
                                                GetMovieClient<IMovieSearcher>(settings),
                                                GetMovieClient<IMovieRequester>(settings),
                                                new DiscordMovieUserInterface(interaction, GetMovieClient<IMovieSearcher>(settings)),
                                                CreateMovieNotificationWorkflow(interaction, settings));
        }

        public IMovieNotificationWorkflow CreateNotificationWorkflow(DiscordInteraction interaction)
        {
            var settings = _settingsProvider.Provide();
            return CreateMovieNotificationWorkflow(interaction, settings);
        }

        public MovieNotificationEngine CreateMovieNotificationEngine(DiscordClient client, ILogger logger)
        {
            var settings = _settingsProvider.Provide();

            IMovieNotifier movieNotifier = null;

            if (settings.NotificationMode == NotificationMode.PrivateMessage)
            {
                movieNotifier = new PrivateMessageMovieNotifier(client, logger);
            }
            else if (settings.NotificationMode == NotificationMode.Channels)
            {
                movieNotifier = new ChannelMovieNotifier(client, settings.NotificationChannels.Select(x => ulong.Parse(x)).ToArray(), logger);
            }
            else
            {
                throw new Exception($"Could not create movie notifier of type \"{settings.NotificationMode}\"");
            }

            return new MovieNotificationEngine(GetMovieClient<IMovieSearcher>(settings), movieNotifier, logger, _notificationsRepository);
        }

        /// <summary>
        /// Direct method to search for movies without requiring a Discord interaction
        /// </summary>
        /// <param name="request">The movie request object</param>
        /// <param name="movieName">The name of the movie to search for</param>
        /// <returns>A list of movies matching the search query</returns>
        public async Task<IReadOnlyList<Movie>> SearchDirectAsync(MovieRequest request, string movieName)
        {
            var settings = _settingsProvider.Provide();
            var searcher = GetMovieClient<IMovieSearcher>(settings);
            
            if (searcher == null)
            {
                throw new InvalidOperationException("No movie searcher available");
            }
            
            return await searcher.SearchMovieAsync(request, movieName);
        }

        /// <summary>
        /// Gets the appropriate movie client based on the current settings
        /// </summary>
        /// <typeparam name="T">The type of client to return</typeparam>
        /// <returns>The movie client</returns>
        public T GetMovieSearcher<T>() where T : class
        {
            var settings = _settingsProvider.Provide();
            return GetMovieClient<T>(settings);
        }
        
        /// <summary>
        /// Gets the IMovieSearcher implementation for the current settings
        /// </summary>
        /// <returns>The IMovieSearcher implementation</returns>
        public IMovieSearcher GetMovieSearcher()
        {
            var settings = _settingsProvider.Provide();
            return GetMovieClient<IMovieSearcher>(settings);
        }
        
        /// <summary>
        /// Gets the IMovieRequester implementation for the current settings
        /// </summary>
        /// <returns>The IMovieRequester implementation</returns>
        public IMovieRequester GetMovieRequester()
        {
            var settings = _settingsProvider.Provide();
            return GetMovieClient<IMovieRequester>(settings);
        }
        
        /// <summary>
        /// Create a notification workflow for Telegram users
        /// </summary>
        /// <param name="user">The user requesting the movie</param>
        /// <returns>A movie notification workflow</returns>
        public IMovieNotificationWorkflow CreateNotificationWorkflowForTelegram(MovieUserRequester user)
        {
            var settings = _settingsProvider.Provide();
            return new MovieNotificationWorkflow(
                _notificationsRepository,
                new NullMovieUserInterface(), // We'll handle notifications directly in Telegram
                GetMovieClient<IMovieSearcher>(settings),
                settings.AutomaticallyNotifyRequesters);
        }

        private IMovieNotificationWorkflow CreateMovieNotificationWorkflow(DiscordInteraction interaction, DiscordSettings settings)
        {
            var userInterface = new DiscordMovieUserInterface(interaction, GetMovieClient<IMovieSearcher>(settings));
            IMovieNotificationWorkflow movieNotificationWorkflow = new DisabledMovieNotificationWorkflow(userInterface);

            if (settings.NotificationMode != NotificationMode.Disabled)
            {
                movieNotificationWorkflow = new MovieNotificationWorkflow(_notificationsRepository, userInterface, GetMovieClient<IMovieSearcher>(settings), settings.AutomaticallyNotifyRequesters);
            }

            return movieNotificationWorkflow;
        }

        private T GetMovieClient<T>(DiscordSettings settings) where T : class
        {
            if (settings.MovieDownloadClient == DownloadClient.Radarr)
            {
                return _radarrDownloadClient as T;
            }
            else if (settings.MovieDownloadClient == DownloadClient.Ombi)
            {
                return _ombiDownloadClient as T;
            }
            else if (settings.MovieDownloadClient == DownloadClient.Overseerr)
            {
                return _overseerrClient as T;
            }
            else
            {
                throw new Exception($"Invalid configured movie download client {settings.MovieDownloadClient}");
            }
        }
    }
}