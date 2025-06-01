
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Askarr.WebApi.AskarrBot.TvShows
{
    public interface ITvShowIssueRequester
    {
        Task<bool> SubmitTvShowIssueAsync(TvShowRequest request, int theTvDbId, string issueValue, string issueDescription);
    }
}