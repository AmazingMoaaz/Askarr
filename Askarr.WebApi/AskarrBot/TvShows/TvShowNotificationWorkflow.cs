﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Askarr.WebApi.AskarrBot.Notifications.TvShows;

namespace Askarr.WebApi.AskarrBot.TvShows
{
    public class TvShowNotificationWorkflow : ITvShowNotificationWorkflow
    {
        private readonly TvShowNotificationsRepository _notificationsRepository;
        private readonly ITvShowUserInterface _userInterface;
        private readonly ITvShowSearcher _tvShowSearcher;
        private readonly bool _automaticNotificationForNewRequests;

        public TvShowNotificationWorkflow(
        TvShowNotificationsRepository movieNotificationsRepository,
        ITvShowUserInterface userInterface,
        ITvShowSearcher tvShowSearcher,
        bool automaticNotificationForNewRequests)
        {
            _notificationsRepository = movieNotificationsRepository;
            _userInterface = userInterface;
            _tvShowSearcher = tvShowSearcher;
            _automaticNotificationForNewRequests = automaticNotificationForNewRequests;
        }

        public Task NotifyForNewRequestAsync(string userId, TvShow tvShow, TvSeason selectedSeason)
        {
            if (_automaticNotificationForNewRequests)
            {
                _notificationsRepository.AddSeasonNotification(userId, tvShow.TheTvDbId, selectedSeason);
            }

            return Task.CompletedTask;
        }

        public async Task NotifyForExistingRequestAsync(string userId, TvShow tvShow, TvSeason selectedSeason)
        {
            if (_notificationsRepository.HasSeasonNotification(userId, tvShow.TheTvDbId, selectedSeason))
            {
                await _userInterface.WarnAlreadyNotifiedForSeasonsAsync(tvShow, selectedSeason);
            }
            else
            {
                await _userInterface.AskForSeasonNotificationRequestAsync(tvShow, selectedSeason);
            }
        }

        public async Task AddNotificationAsync(string userId, int theTvDbId, string seasonType, int seasonNumber)
        {
            TvSeason selectedSeason;

            var tvShow = await _tvShowSearcher.GetTvShowDetailsAsync(new TvShowRequest(null, int.MinValue), theTvDbId);

            switch (seasonType.ToLower())
            {
                case "f":
                    selectedSeason = new FutureTvSeasons { SeasonNumber = seasonNumber };
                    break;
                case "a":
                    selectedSeason = new AllTvSeasons { SeasonNumber = seasonNumber };
                    break;
                case "n":
                    selectedSeason = new NormalTvSeason { SeasonNumber = seasonNumber };
                    break;
                default:
                    throw new Exception($"Could not handle season of type \"{seasonType}\" when adding notifications");
            }

            _notificationsRepository.AddSeasonNotification(userId, theTvDbId, selectedSeason);
            await _userInterface.DisplayNotificationSuccessForSeasonAsync(tvShow, selectedSeason);
        }
    }
}