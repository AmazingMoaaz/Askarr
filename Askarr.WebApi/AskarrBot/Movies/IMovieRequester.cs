using System.Threading.Tasks;

namespace Askarr.WebApi.AskarrBot.Movies
{
    public interface IMovieRequester
    {
        Task<MovieRequestResult> RequestMovieAsync(MovieRequest request, Movie movie);
    }

    public class MovieRequestResult
    {
        public bool WasDenied { get; set; }
    }
}