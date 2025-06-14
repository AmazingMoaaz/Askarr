﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Askarr.WebApi.AskarrBot.ChatClients.Discord;
using Askarr.WebApi.AskarrBot.DownloadClients;
using Askarr.WebApi.AskarrBot.Locale;
using Askarr.WebApi.AskarrBot.TvShows;

namespace Askarr.WebApi.AskarrBot.Notifications.TvShows
{
    public class PrivateMessageTvShowNotifier : ITvShowNotifier
    {
        private readonly DiscordClient _discordClient;
        private readonly DiscordSettingsProvider _discordSettingsProvider;
        private readonly ILogger _logger;

        public PrivateMessageTvShowNotifier(
            DiscordClient discordClient,
            DiscordSettingsProvider discordSettingsProvider,
            ILogger logger)
        {
            _discordClient = discordClient;
            _discordSettingsProvider = discordSettingsProvider;
            _logger = logger;
        }

        public async Task<HashSet<string>> NotifyAsync(IReadOnlyCollection<string> userIds, TvShow tvShow, int seasonNumber, CancellationToken token)
        {
            var userNotified = new HashSet<string>();

            if (_discordClient.Guilds.Any())
            {
                foreach (var userId in userIds)
                {
                    if (token.IsCancellationRequested)
                        return userNotified;

                    try
                    {
                        DiscordMember user = null;

                        foreach (var guild in _discordClient.Guilds.Values)
                        {
                            try
                            {
                                user = await guild.GetMemberAsync(ulong.Parse(userId));
                                break;
                            }
                            catch { }
                        }

                        if (user != null)
                        {
                            var channel = await user.CreateDmChannelAsync();

                            if (_discordSettingsProvider.Provide().TvShowDownloadClient == DownloadClient.Overseerr)
                                await channel.SendMessageAsync(Language.Current.DiscordNotificationTvDMSeason.ReplaceTokens(tvShow, seasonNumber), DiscordTvShowUserInterface.GenerateTvShowDetailsAsync(tvShow));
                            else
                                await channel.SendMessageAsync(Language.Current.DiscordNotificationTvDMFirstEpisode.ReplaceTokens(tvShow, seasonNumber), DiscordTvShowUserInterface.GenerateTvShowDetailsAsync(tvShow));
                        }
                        else
                        {
                            _logger.LogWarning($"Removing tv show notification for user with ID {userId} as it could not be found in any of the guilds.");
                        }

                        userNotified.Add(userId);
                    }
                    catch (DSharpPlus.Exceptions.UnauthorizedException ex)
                    {
                        userNotified.Add(userId);
                        _logger.LogWarning($"Removing tv show notification [{tvShow.Title}] for user [{userId}] as we are missing permissions to do so [Unauthorized].");
                    }
                    catch (System.Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while sending a tv show notification to a specific user: " + ex.Message);
                    }
                }
            }

            return userNotified;
        }
    }
}