using System.Threading.Tasks;


namespace Askarr.WebApi.AskarrBot.Movies
{
    public interface IMovieIssueRequester
    {
        Task<bool> SubmitMovieIssueAsync(MovieRequest request, int theMovieDbId, string issueValue, string issueDescription);
    }
}
