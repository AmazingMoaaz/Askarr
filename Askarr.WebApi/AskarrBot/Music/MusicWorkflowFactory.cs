﻿using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Askarr.WebApi.AskarrBot.ChatClients.Discord;
using Askarr.WebApi.AskarrBot.DownloadClients;
using Askarr.WebApi.AskarrBot.DownloadClients.Lidarr;
using Askarr.WebApi.AskarrBot.Notifications;
using Askarr.WebApi.AskarrBot.Notifications.Music;
using System;
using System.Linq;

namespace Askarr.WebApi.AskarrBot.Music
{
    public class MusicWorkflowFactory
    {
        private readonly DiscordSettingsProvider _settingsProvider;
        private readonly MusicNotificationsRepository _notificationsRepository;
        private LidarrClient _lidarrClient;


        public MusicWorkflowFactory(
            DiscordSettingsProvider settingsProvider,
            MusicNotificationsRepository musicNotificationsRepository,
            LidarrClient lidarrClient
        )
        {
            _settingsProvider = settingsProvider;
            _notificationsRepository = musicNotificationsRepository;
            _lidarrClient = lidarrClient;
        }


        public MusicRequestingWorkflow CreateRequestingWorkflow(DiscordInteraction interaction, int categoryId)
        {
            DiscordSettings settings = _settingsProvider.Provide();
            return new MusicRequestingWorkflow(
                new MusicUserRequester(
                    interaction.User.Id.ToString(),
                    interaction.User.Username
                    ),
                categoryId,
                GetMusicClient<IMusicSearcher>(settings),
                GetMusicClient<IMusicRequester>(settings),
                new DiscordMusicUserInterface(interaction, GetMusicClient<IMusicSearcher>(settings)),
                CreateMusicNotificationWorkflow(interaction, settings)
                );
        }



        public IMusicNotificationWorkflow CreateNotificationWorkflow(DiscordInteraction interaction)
        {
            DiscordSettings settings = _settingsProvider.Provide();
            return CreateMusicNotificationWorkflow(interaction, settings);
        }


        public MusicNotificationEngine CreateMusicNotificationEngine(DiscordClient client, ILogger logger)
        {
            DiscordSettings settings = _settingsProvider.Provide();
            IMusicNotifier musicNotifier = null;

            if (settings.NotificationMode == NotificationMode.PrivateMessage)
                musicNotifier = new PrivateMessageMusicNotifier(client, logger);
            else if (settings.NotificationMode == NotificationMode.Channels)
                musicNotifier = new ChannelMusicNotifier(client, settings.NotificationChannels.Select(x => ulong.Parse(x)).ToArray(), logger);
            else
                throw new Exception($"Could not create music artist notifier of type \"{settings.NotificationMode}\"");

            return new MusicNotificationEngine(GetMusicClient<IMusicSearcher>(settings), musicNotifier, logger, _notificationsRepository);
        }



        private IMusicNotificationWorkflow CreateMusicNotificationWorkflow(DiscordInteraction interaction, DiscordSettings settings)
        {
            DiscordMusicUserInterface userInterface = new DiscordMusicUserInterface(interaction, GetMusicClient<IMusicSearcher>(settings));
            IMusicNotificationWorkflow musicNotificationWorkflow = new DisabledMusicNotificationWorkflow(userInterface);

            if (settings.NotificationMode != NotificationMode.Disabled)
                musicNotificationWorkflow = new MusicNotificationWorkflow(_notificationsRepository, userInterface, GetMusicClient<IMusicSearcher>(settings), settings.AutomaticallyNotifyRequesters);

            return musicNotificationWorkflow;
        }



        private T GetMusicClient<T>(DiscordSettings settings) where T : class
        {
            if (settings.MusicDownloadClient == DownloadClient.Lidarr)
            {
                return _lidarrClient as T;
            }

            throw new Exception($"Invalid configured music download client {settings.MusicDownloadClient}");
        }
    }
}
