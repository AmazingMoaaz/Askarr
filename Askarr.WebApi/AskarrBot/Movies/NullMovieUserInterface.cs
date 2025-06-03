using System.Collections.Generic;
using System.Threading.Tasks;

namespace Askarr.WebApi.AskarrBot.Movies
{
    /// <summary>
    /// A null implementation of IMovieUserInterface that does nothing
    /// Used when we don't need to interact directly with the user through this interface
    /// </summary>
    public class NullMovieUserInterface : IMovieUserInterface
    {
        public Task AskForNotificationRequestAsync(Movie movie) => Task.CompletedTask;
        public Task CompleteMovieIssueModalRequestAsync(Movie movie, bool success) => Task.CompletedTask;
        public Task ConfirmRequestedMovieAsync(Movie movie) => Task.CompletedTask;
        public Task ConfirmSelectedIssueAsync(string issue) => Task.CompletedTask;
        public Task ConfirmSelectedMovieAsync(Movie movie) => Task.CompletedTask;
        public Task DisplayMovieDetailsAsync(MovieRequest request, Movie movie) => Task.CompletedTask;
        public Task DisplayMovieIssueDetailsAsync(MovieRequest request, Movie movie, string issue) => Task.CompletedTask;
        public Task DisplayMovieIssueModalAsync(MovieRequest request, Movie movie, string issue) => Task.CompletedTask;
        public Task DisplayNotificationSuccessAsync(Movie movie) => Task.CompletedTask;
        public Task DisplayRequestDeniedAsync(Movie movie) => Task.CompletedTask;
        public Task DisplayRequestSuccessAsync(Movie movie) => Task.CompletedTask;
        public Task ShowIssueSelectionAsync(MovieRequest request, IReadOnlyDictionary<string, int> issueTypes) => Task.CompletedTask;
        public Task ShowMovieIssueSelection(MovieRequest request, IReadOnlyList<Movie> movies) => Task.CompletedTask;
        public Task ShowMovieSelection(MovieRequest request, IReadOnlyList<Movie> movies) => Task.CompletedTask;
        public Task WarnAlreadyRequestedAsync(Movie movie) => Task.CompletedTask;
        public Task WarnMovieAlreadyAvailableAsync(Movie movie) => Task.CompletedTask;
        public Task WarnMovieAlreadyRequestedAsync(Movie movie) => Task.CompletedTask;
        public Task WarnMovieUnavailableAndAlreadyHasNotificationAsync(Movie movie) => Task.CompletedTask;
        public Task WarnNoMovieFoundAsync(string movieName) => Task.CompletedTask;
        public Task WarnNoMovieFoundByTheMovieDbIdAsync(string theMovieDbId) => Task.CompletedTask;
        public Task WarnUnavailableMovieAsync(Movie movie) => Task.CompletedTask;
    }
} 