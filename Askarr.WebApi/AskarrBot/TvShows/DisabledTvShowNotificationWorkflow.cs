﻿using System.Threading.Tasks;

namespace Askarr.WebApi.AskarrBot.TvShows
{
    public class DisabledTvShowNotificationWorkflow : ITvShowNotificationWorkflow
    {
        private readonly ITvShowUserInterface _userInterface;

        public DisabledTvShowNotificationWorkflow(ITvShowUserInterface userInterface)
        {
            _userInterface = userInterface;
        }

        public Task NotifyForNewRequestAsync(string userId, TvShow tvShow, TvSeason selectedSeason)
        {
            return Task.CompletedTask;
        }

        public Task NotifyForExistingRequestAsync(string userId, TvShow tvShow, TvSeason selectedSeason)
        {
            return _userInterface.WarnAlreadySeasonAlreadyRequestedAsync(tvShow, selectedSeason);
        }

        public Task AddNotificationAsync(string userId, int theTvDbId, string seasonType, int seasonNumber)
        {
            return Task.CompletedTask;
        }
    }
}