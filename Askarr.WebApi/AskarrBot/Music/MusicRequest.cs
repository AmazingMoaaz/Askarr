using Askarr.WebApi.AskarrBot.Movies;

namespace Askarr.WebApi.AskarrBot.Music
{
    public class MusicRequest
    {
        public int CategoryId { get; }
        public MusicUserRequester User { get; }

        public MusicRequest(MusicUserRequester user, int categoryId)
        {
            User = user;
            CategoryId = categoryId;
        }
    }
}
