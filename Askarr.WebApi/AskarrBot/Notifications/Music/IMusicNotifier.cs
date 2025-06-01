using Microsoft.Extensions.Primitives;
using Askarr.WebApi.AskarrBot.Music;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Askarr.WebApi.AskarrBot.Notifications.Music
{
    public interface IMusicNotifier
    {
        Task<HashSet<string>> NotifyArtistAsync(IReadOnlyCollection<string> userIds, MusicArtist musicArtist, CancellationToken token);
    }
}
