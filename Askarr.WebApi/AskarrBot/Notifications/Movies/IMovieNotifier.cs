using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Askarr.WebApi.AskarrBot.Movies;

namespace Askarr.WebApi.AskarrBot.Notifications.Movies
{
    public interface IMovieNotifier
    {
        Task<HashSet<string>> NotifyAsync(IReadOnlyCollection<string> userIds, Movie movie, CancellationToken token);
    }
}