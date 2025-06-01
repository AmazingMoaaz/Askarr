using System.Collections.Generic;
using System.Threading.Tasks;

namespace Askarr.WebApi.AskarrBot.Movies
{
    public interface IMovieSearcher
    {
        Task<IReadOnlyList<Movie>> SearchMovieAsync(MovieRequest request, string movieName);
        Task<MovieDetails> GetMovieDetails(MovieRequest request, string theMovieDbId);
        Task<Dictionary<int, Movie>> SearchAvailableMoviesAsync(HashSet<int> movies, System.Threading.CancellationToken token);
        Task<Movie> SearchMovieAsync(MovieRequest request, int theMovieDbId);
    }

    public class MovieDetails
    {
        public string InTheatersDate { get; set; }
        public string PhysicalReleaseName { get; set; }
        public string PhysicalReleaseDate { get; set; }
    }
}