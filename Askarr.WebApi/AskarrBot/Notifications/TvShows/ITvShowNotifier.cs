using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Askarr.WebApi.AskarrBot.TvShows;

namespace Askarr.WebApi.AskarrBot.Notifications.TvShows
{
    public interface ITvShowNotifier
    {
        Task<HashSet<string>> NotifyAsync(IReadOnlyCollection<string> userIds, TvShow tvShow, int seasonNumber, CancellationToken token);
    }
}