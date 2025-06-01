using Askarr.WebApi.AskarrBot.Movies;
using System.Threading.Tasks;

namespace Askarr.WebApi.AskarrBot.Music
{
    public interface IMusicNotificationWorkflow
    {
        Task NotifyForNewRequestAsync(string userId, MusicArtist musicArtist);
        Task NotifyForExistingRequestAsync(string userId, MusicArtist musicArtist);
        Task AddNotificationArtistAsync(string userId, string musicArtistId);
    }
}
