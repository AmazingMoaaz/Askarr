using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Askarr.WebApi.AskarrBot.Music
{
    public interface IMusicSearcher
    {
        Task<IReadOnlyList<MusicArtist>> SearchMusicForArtistAsync(MusicRequest request, string artistName);
        Task<MusicArtist> SearchMusicForArtistIdAsync(MusicRequest request, string artistId);


        Task<Dictionary<string, MusicArtist>> SearchAvailableMusicArtistAsync(HashSet<string> artistIds, CancellationToken token);
    }
}
