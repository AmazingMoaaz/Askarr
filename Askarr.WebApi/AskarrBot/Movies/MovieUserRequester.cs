namespace Askarr.WebApi.AskarrBot.Movies
{
    public class MovieUserRequester
    {
        public string UserId { get; }
        public string Username { get; }

        public MovieUserRequester(string userId, string username)
        {
            UserId = userId;
            Username = username;
        }
    }
}