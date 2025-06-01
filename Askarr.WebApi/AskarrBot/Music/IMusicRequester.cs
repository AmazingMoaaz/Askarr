using System.Threading.Tasks;

namespace Askarr.WebApi.AskarrBot.Music
{
    public interface IMusicRequester
    {
        Task<MusicRequestResult> RequestMusicAsync(MusicRequest request, MusicArtist music);
    }


    public class MusicRequestResult
    {
        public bool WasDenied { get; set; }
    }
}
