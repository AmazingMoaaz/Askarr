﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Askarr.WebApi.Extensions;
using Askarr.WebApi.AskarrBot.Movies;
using Askarr.WebApi.AskarrBot.TvShows;
using static Askarr.WebApi.AskarrBot.DownloadClients.Ombi.OmbiClient;

namespace Askarr.WebApi.AskarrBot.DownloadClients.Ombi
{
    public class OmbiClient : IMovieRequester, IMovieSearcher, ITvShowSearcher, ITvShowRequester, IMovieIssueSearcher, IMovieIssueRequester, ITvShowIssueSearcher, ITvShowIssueRequester
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OmbiClient> _logger;
        private OmbiSettingsProvider _ombiSettingsProvider;
        private OmbiSettings OmbiSettings => _ombiSettingsProvider.Provide();
        private string BaseURL => GetBaseURL(OmbiSettings);

        public Dictionary<string, int> IssueTypes => GetOmbiIssueCategoriesAsync();

        public OmbiClient(IHttpClientFactory httpClientFactory, ILogger<OmbiClient> logger, OmbiSettingsProvider OmbiSettingsProvider)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _ombiSettingsProvider = OmbiSettingsProvider;
        }

        public static async Task TestConnectionAsync(HttpClient httpClient, ILogger<OmbiClient> logger, OmbiSettings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.BaseUrl) && !settings.BaseUrl.StartsWith("/"))
            {
                throw new Exception("Invalid base URL, must start with /");
            }

            var testSuccessful = false;

            try
            {
                var response = await HttpGetAsync(httpClient, settings, $"{GetBaseURL(settings)}/api/v1/Settings/ombi");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new Exception("Invalid api key");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Invalid host and/or port");
                }

                try
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JObject.Parse(responseString);

                    if (!jsonResponse.baseUrl.ToString().Equals(settings.BaseUrl, StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exception("Base url does not match what is set in Ombi");
                    }
                }
                catch
                {
                    throw new Exception("Base url does not match what is set in Ombi");
                }

                testSuccessful = true;
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Error while testing Ombi connection: " + ex.Message);
                throw new Exception("Invalid host and/or port");
            }
            catch (System.Exception ex)
            {
                logger.LogWarning(ex, "Error while testing Ombi connection: " + ex.Message);

                if (ex.GetType() == typeof(System.Exception))
                {
                    throw;
                }
                else
                {
                    throw new Exception("Invalid host and/or port");
                }
            }

            if (!testSuccessful)
            {
                throw new Exception("Invalid host and/or port");
            }
        }

        public async Task<MovieRequestResult> RequestMovieAsync(MovieRequest request, Movie movie)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var ombiUser = await FindLinkedOmbiUserAsync(request.User.UserId, request.User.Username);

                    if (ombiUser.CanRequestMovies && ombiUser.MoviesQuotaRemaining > 0)
                    {
                        var response = await HttpPostAsync(ombiUser, $"{BaseURL}/api/v1/Request/Movie", JsonConvert.SerializeObject(new { theMovieDbId = movie.TheMovieDbId }));
                        await response.ThrowIfNotSuccessfulAsync("OmbiCreateMovieRequest failed", x => x.error);

                        return new MovieRequestResult();
                    }

                    return new MovieRequestResult
                    {
                        WasDenied = true
                    };
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"An error while requesting movie \"{movie.Title}\" from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while requesting a movie from oOmbimbi");
        }

        public async Task<Movie> SearchMovieAsync(MovieRequest request, int theMovieDbId)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var response = await HttpGetAsync($"{BaseURL}/api/v1/search/movie/info/{theMovieDbId}");
                    await response.ThrowIfNotSuccessfulAsync("OmbiMovieSearchByMovieDbId failed", x => x.error);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonMovie = JsonConvert.DeserializeObject<JSONMovie>(jsonResponse);

                    var convertedMovie = Convert(jsonMovie);
                    return convertedMovie?.TheMovieDbId == theMovieDbId.ToString() ? convertedMovie : null;
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while searching for a movie by tmdbId \"{theMovieDbId}\" from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while searching for a movie by tmdbId from Ombi");
        }

        public async Task<IReadOnlyList<Movie>> SearchMovieAsync(MovieRequest request, string movieName)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var response = await HttpGetAsync($"{BaseURL}/api/v1/search/movie/{movieName}");
                    await response.ThrowIfNotSuccessfulAsync("OmbiMovieSearch failed", x => x.error);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var movies = JsonConvert.DeserializeObject<List<JSONMovie>>(jsonResponse).ToArray();

                    return movies.Select(Convert).ToArray();
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while searching for movies from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while searching for movies from Ombi");
        }

        public Task<MovieDetails> GetMovieDetails(MovieRequest request, string theMovieDbId)
        {
            return TheMovieDb.GetMovieDetailsAsync(_httpClientFactory.CreateClient(), theMovieDbId, _logger);
        }

        public async Task<Movie> GetMovieAsync(int movieId)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var response = await HttpGetAsync($"{BaseURL}/api/v1/search/movie/info/{movieId.ToString()}");
                    await response.ThrowIfNotSuccessfulAsync("OmbiGetMovieInfo failed", x => x.error);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonMovie = JsonConvert.DeserializeObject<JSONMovie>(jsonResponse);

                    return Convert(jsonMovie);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while searching for a movie from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while searching for a movie from Ombi");
        }

        public async Task<Dictionary<int, Movie>> SearchAvailableMoviesAsync(HashSet<int> movieIds, System.Threading.CancellationToken token)
        {
            var retryCount = 0;

            while (retryCount <= 5 && !token.IsCancellationRequested)
            {
                try
                {
                    var movies = new HashSet<Movie>();

                    foreach (var movieId in movieIds)
                    {
                        await Task.Delay(100);
                        var movie = await GetMovieAsync(movieId);

                        if (movie.Available)
                        {
                            movies.Add(movie);
                        }
                    }

                    return movies.ToDictionary(x => int.Parse(x.TheMovieDbId), x => x);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while searching for availables movies from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000, token);
                }
            }

            throw new System.Exception("An error occurred while searching for availables movies from Ombi");
        }

        public async Task<TvShowRequestResult> RequestTvShowAsync(TvShowRequest request, TvShow tvShow, TvSeason season)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var ombiUser = await FindLinkedOmbiUserAsync(request.User.UserId, request.User.Username);

                    var jsonTvShow = await FindTvShowByTheTvDbIdAsync(tvShow.TheTvDbId.ToString());

                    var wantedSeasonIds = season is AllTvSeasons
                        ? new HashSet<int>(tvShow.Seasons.OfType<NormalTvSeason>().Select(x => x.SeasonNumber))
                        : season is FutureTvSeasons
                            ? new HashSet<int>()
                            : new HashSet<int> { season.SeasonNumber };

                    var episodeToRequestCount = jsonTvShow.seasonRequests.Sum(s => wantedSeasonIds.Contains(s.seasonNumber) && s.CanBeRequested() ? s.episodes.Where(x => x.CanBeRequested()).Count() : 0);

                    if (ombiUser.CanRequestTvShows && ombiUser.TvEpisodeQuotaRemaining > 0)
                    {
                        var response = await HttpPostAsync(ombiUser, $"{BaseURL}/api/v1/Request/Tv", JsonConvert.SerializeObject(new
                        {
                            tvDbId = tvShow.TheTvDbId,
                            requestAll = false,
                            latestSeason = false,
                            firstSeason = false,
                            seasons = jsonTvShow.seasonRequests.Select(s =>
                            {
                                var episodes = wantedSeasonIds.Contains(s.seasonNumber) && s.CanBeRequested() ? s.episodes.Where(x => x.CanBeRequested()).Select(e => new JSONTvEpisode { episodeNumber = e.episodeNumber }) : Array.Empty<JSONTvEpisode>();
                                episodes = episodes.Take(ombiUser.TvEpisodeQuotaRemaining);
                                ombiUser.TvEpisodeQuotaRemaining -= episodes.Count();

                                return new
                                {
                                    seasonNumber = s.seasonNumber,
                                    episodes = episodes,
                                };
                            }),
                        }));

                        await response.ThrowIfNotSuccessfulAsync("OmbiCreateTvShowRequest failed", x => x.error);

                        return new TvShowRequestResult();
                    }

                    return new TvShowRequestResult
                    {
                        WasDenied = true
                    };
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"An error while requesting tv show \"{tvShow.Title}\" from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("Something went wrong while requesting a tv show from Ombi");
        }

        public async Task<TvShow> GetTvShowDetailsAsync(TvShowRequest request, int TheTvDbId)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var jsonTvShow = await FindTvShowByTheTvDbIdAsync(TheTvDbId.ToString());
                    return Convert(jsonTvShow);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while getting details for a tv show from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while getting details for a tv show from Ombi");
        }

        public async Task<IReadOnlyList<TvShow>> GetTvShowDetailsAsync(HashSet<int> tvShowIds, System.Threading.CancellationToken token)
        {
            var retryCount = 0;

            while (retryCount <= 5 && !token.IsCancellationRequested)
            {
                try
                {
                    var tvShows = new List<TvShow>();

                    foreach (var showId in tvShowIds)
                    {
                        await Task.Delay(100);

                        try
                        {
                            var show = await GetTvShowDetailsAsync(new TvShowRequest(null, int.MinValue), showId);
                            tvShows.Add(show);
                        }
                        catch
                        {
                            // Ignore
                        }
                    }

                    return tvShows.Where(x => x != null).ToArray();
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while getting tv show details from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000, token);
                }
            }

            throw new System.Exception("An error occurred while getting tv show details from Ombi");
        }


        public async Task<SearchedTvShow> SearchTvShowAsync(TvShowRequest request, int tvDbId)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var response = await HttpGetAsync($"{BaseURL}/api/v1/Search/Tv/info/{tvDbId}");
                    await response.ThrowIfNotSuccessfulAsync("OmbiSearchTvShowByTvDbId failed", x => x.error);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonTvShow = JsonConvert.DeserializeObject<JSONTvShow>(jsonResponse);

                    var searchedTvShow = new SearchedTvShow
                    {
                        Title = jsonTvShow.title,
                        Banner = jsonTvShow.banner,
                        TheTvDbId = jsonTvShow.id,
                        FirstAired = jsonTvShow.firstAired,
                    };

                    return searchedTvShow.TheTvDbId == tvDbId ? searchedTvShow : null;
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while searching for tv show by tvDbId \"{tvDbId}\" from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while searching for tv show by tvDbId from Ombi");
        }

        public async Task<IReadOnlyList<SearchedTvShow>> SearchTvShowAsync(TvShowRequest request, string tvShowName)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var response = await HttpGetAsync($"{BaseURL}/api/v1/Search/Tv/{tvShowName}");

                    await response.ThrowIfNotSuccessfulAsync("OmbiSearchTvShow failed", x => x.error);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonTvShows = JsonConvert.DeserializeObject<List<JSONTvShow>>(jsonResponse).ToArray();

                    return jsonTvShows.Select(x => new SearchedTvShow
                    {
                        Title = x.title,
                        Banner = x.banner,
                        TheTvDbId = x.id,
                        FirstAired = x.firstAired,
                    }).ToArray();
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while searching for tv show \"{tvShowName}\" from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while searching for tv show from Ombi");
        }

        private async Task<JSONTvShow> FindTvShowByTheTvDbIdAsync(string theTvDbId)
        {
            var response = await HttpGetAsync($"{BaseURL}/api/v1/Search/tv/info/{theTvDbId}");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            await response.ThrowIfNotSuccessfulAsync("OmbiFindTvShowTvDbId failed", x => x.error);

            return JsonConvert.DeserializeObject<JSONTvShow>(jsonResponse);
        }

        private Movie Convert(JSONMovie jsonMovie)
        {
            return new Movie
            {
                TheMovieDbId = jsonMovie.theMovieDbId,
                Title = jsonMovie.title,
                Available = jsonMovie.available,
                Quality = jsonMovie.quality,
                Requested = jsonMovie.requested,
                PlexUrl = jsonMovie.plexUrl,
                EmbyUrl = jsonMovie.embyUrl,
                Overview = jsonMovie.overview,
                PosterPath = !string.IsNullOrWhiteSpace(jsonMovie.posterPath) ? $"https://image.tmdb.org/t/p/w500{jsonMovie.posterPath}" : null,
                ReleaseDate = jsonMovie.releaseDate,
            };
        }

        private TvShow Convert(JSONTvShow jsonTvShow)
        {
            if (jsonTvShow == null)
            {
                return null;
            }

            return new TvShow
            {
                TheTvDbId = jsonTvShow.id,
                Title = jsonTvShow.title,
                Quality = jsonTvShow.quality,
                PlexUrl = jsonTvShow.plexUrl,
                EmbyUrl = jsonTvShow.embyUrl,
                Overview = jsonTvShow.overview,
                Banner = jsonTvShow.banner,
                FirstAired = jsonTvShow.firstAired,
                Network = jsonTvShow.network,
                Status = jsonTvShow.status,
                WebsiteUrl = $"https://www.thetvdb.com/?id={jsonTvShow.id}&tab=series",
                HasEnded = !string.IsNullOrWhiteSpace(jsonTvShow.status) && jsonTvShow.status.Equals("ended", StringComparison.InvariantCultureIgnoreCase),
                Seasons = jsonTvShow.seasonRequests.Select(x => new NormalTvSeason
                {
                    SeasonNumber = x.seasonNumber,
                    IsAvailable = x.episodes.FirstOrDefault()?.available == true,
                    IsRequested = x.episodes.All(x => x.CanBeRequested()) ? RequestedState.None : x.episodes.All(x => !x.CanBeRequested()) ? RequestedState.Full : RequestedState.Partial,
                }).ToArray(),
                IsRequested = jsonTvShow.requested || jsonTvShow.available,
            };
        }

        public async Task<OmbiUser> FindLinkedOmbiUserAsync(string requesterUniqueId, string requesterUsername)
        {
            var response = await HttpGetAsync($"{BaseURL}/api/v1/Identity/Users");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            await response.ThrowIfNotSuccessfulAsync("OmbiFindAllUsers failed", x => x.error);

            JSONOmbiUser[] allOmbiUsers = JsonConvert.DeserializeObject<List<JSONOmbiUser>>(jsonResponse).ToArray();
            OmbiUser ombiUser = null;

            ombiUser = await FindLinkedUserByNotificationPreferencesAsync(requesterUniqueId, allOmbiUsers, ombiUser);

            if (ombiUser == null && !string.IsNullOrEmpty(OmbiSettings.ApiUsername))
            {
                ombiUser = FindDefaultApiUserAsync(allOmbiUsers, requesterUsername);

                if (ombiUser == null)
                {
                    return new OmbiUser
                    {
                        Username = "NO_ACCESS",
                        ApiAlias = "NO_ACCESS",
                        CanRequestMovies = false,
                        MoviesQuotaRemaining = -1,
                        CanRequestTvShows = false,
                        TvEpisodeQuotaRemaining = -1,
                    };
                }
            }

            if (ombiUser == null)
            {
                return new OmbiUser
                {
                    Username = "api",
                    ApiAlias = requesterUsername,
                    CanRequestMovies = true,
                    MoviesQuotaRemaining = int.MaxValue,
                    CanRequestTvShows = true,
                    TvEpisodeQuotaRemaining = int.MaxValue,
                };
            }

            return ombiUser;
        }

        private async Task<OmbiUser> FindLinkedUserByNotificationPreferencesAsync(string userUniqueId, JSONOmbiUser[] allOmbiUsers, OmbiUser ombiUser)
        {
            await Task.WhenAll(allOmbiUsers.Select(async x =>
            {
                try
                {
                    var notifResponse = await HttpGetAsync($"{BaseURL}/api/v1/Identity/notificationpreferences/{x.id.ToString()}");
                    var jsonNotifResponse = await notifResponse.Content.ReadAsStringAsync();
                    await notifResponse.ThrowIfNotSuccessfulAsync("OmbiFindUserNotificationPreferences failed", x => x.error);

                    IEnumerable<dynamic> notificationPreferences = JArray.Parse(jsonNotifResponse);
                    var matchingDiscordNotification = notificationPreferences.FirstOrDefault(n => n.agent == 1 && (n.value.ToString().Trim().Equals(userUniqueId.Trim(), StringComparison.InvariantCultureIgnoreCase) || n.value.ToString().Trim().Equals($"<@{userUniqueId.Trim()}>", StringComparison.InvariantCultureIgnoreCase)));

                    if (matchingDiscordNotification != null)
                    {
                        ombiUser = new OmbiUser
                        {
                            Username = x.userName,
                            ApiAlias = !string.IsNullOrWhiteSpace(x.alias) ? x.alias : x.userName,
                            CanRequestMovies = x.CanRequestMovie,
                            MoviesQuotaRemaining = x.movieRequestQuota == null || !x.movieRequestQuota.hasLimit ? int.MaxValue : x.movieRequestQuota.remaining,
                            CanRequestTvShows = x.CanRequestTv,
                            TvEpisodeQuotaRemaining = x.episodeRequestQuota == null || !x.episodeRequestQuota.hasLimit ? int.MaxValue : x.episodeRequestQuota.remaining,
                        };
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }));

            return ombiUser;
        }

        private OmbiUser FindDefaultApiUserAsync(JSONOmbiUser[] allOmbiUsers, string requesterUsername)
        {
            OmbiUser ombiUser = null;

            var defaultApiUser = allOmbiUsers.FirstOrDefault(x => x.userName.Equals(OmbiSettings.ApiUsername, StringComparison.InvariantCultureIgnoreCase));

            if (defaultApiUser != null)
            {
                ombiUser = new OmbiUser
                {
                    Username = defaultApiUser.userName,
                    ApiAlias = requesterUsername,
                    CanRequestMovies = defaultApiUser.IsDisabled ? false : defaultApiUser.CanRequestMovie,
                    MoviesQuotaRemaining = defaultApiUser.movieRequestQuota == null || !defaultApiUser.movieRequestQuota.hasLimit ? int.MaxValue : defaultApiUser.movieRequestQuota.remaining,
                    CanRequestTvShows = defaultApiUser.IsDisabled ? false : defaultApiUser.CanRequestTv,
                    TvEpisodeQuotaRemaining = defaultApiUser.episodeRequestQuota == null || !defaultApiUser.episodeRequestQuota.hasLimit ? int.MaxValue : defaultApiUser.episodeRequestQuota.remaining,
                };
            }

            return ombiUser;
        }

        private static string GetBaseURL(OmbiSettings settings)
        {
            var protocol = settings.UseSSL ? "https" : "http";
            return $"{protocol}://{settings.Hostname}:{settings.Port}{settings.BaseUrl}";
        }

        private Task<HttpResponseMessage> HttpGetAsync(string url)
        {
            return HttpGetAsync(_httpClientFactory.CreateClient(), OmbiSettings, url);
        }

        private static async Task<HttpResponseMessage> HttpGetAsync(HttpClient client, OmbiSettings settings, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("ApiKey", settings.ApiKey);

            return await client.SendAsync(request);
        }

        private async Task<HttpResponseMessage> HttpPostAsync(OmbiUser ombiUser, string url, string content)
        {

            var postRequest = new StringContent(content);
            postRequest.Headers.Clear();
            postRequest.Headers.Add("Content-Type", "application/json");
            postRequest.Headers.Add("ApiKey", OmbiSettings.ApiKey);
            
            if (ombiUser != null)
            {
                var apiAlias = Sanitize(ombiUser.ApiAlias);
                var apiUsername = Sanitize(ombiUser.Username);
                postRequest.Headers.Add("ApiAlias", !string.IsNullOrWhiteSpace(apiAlias) ? apiAlias : "Unknown");
                postRequest.Headers.Add("UserName", !string.IsNullOrWhiteSpace(apiUsername) ? apiUsername : "api");
            }

            var client = _httpClientFactory.CreateClient();
            return await client.PostAsync(url, postRequest);
        }

        private string Sanitize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return Regex.Replace(value, @"[^\u0000-\u007F]+", string.Empty);
        }


        /// <summary>
        /// Handles the fetching of all categories form the Ombi API
        /// </summary>
        /// <returns>Returns Dictionary of string and int, string is the name of the categorie and int is the ID</returns>
        private Dictionary<string, int> GetOmbiIssueCategoriesAsync()
        {
            HttpResponseMessage response = HttpGetAsync($"{BaseURL}/api/v1/Issues/categories").Result;
            string jsonResponse = response.Content.ReadAsStringAsync().Result;
            _ = response.ThrowIfNotSuccessfulAsync("OmbiFindUserNotificationPreferences failed", x => x.error);

            List<JSONIssueCategorie> notificationPreferences = JsonConvert.DeserializeObject<List<JSONIssueCategorie>>(jsonResponse);
            Dictionary<string, int> categories = new Dictionary<string, int>();
            foreach (JSONIssueCategorie i in notificationPreferences)
            {
                categories.Add(i.CategorieName, i.Id);
            }

            return categories;
        }



        /// <summary>
        /// Handles the searching for movies currently in library and match movie name
        /// </summary>
        /// <param name="request"></param>
        /// <param name="movieName"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<IReadOnlyList<Movie>> SearchMovieLibraryAsync(MovieRequest request, string movieName)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var response = await HttpGetAsync($"{BaseURL}/api/v1/search/movie/{movieName}");
                    await response.ThrowIfNotSuccessfulAsync("OmbiMovieSearch failed", x => x.error);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var movies = JsonConvert.DeserializeObject<List<JSONMovie>>(jsonResponse)
                        .Where(x => x.available)
                        .ToArray();

                    return movies.Select(Convert).ToArray();
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while searching for movies from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while searching for movies from Ombi");
        }



        /// <summary>
        /// Handles the searching for Movie DB ID and checking it is in the current library
        /// </summary>
        /// <param name="request"></param>
        /// <param name="theMovieDbId"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<Movie> SearchMovieLibraryAsync(MovieRequest request, int theMovieDbId)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var response = await HttpGetAsync($"{BaseURL}/api/v1/search/movie/info/{theMovieDbId}");
                    await response.ThrowIfNotSuccessfulAsync("OmbiMovieSearchByMovieDbId failed", x => x.error);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonMovie = JsonConvert.DeserializeObject<JSONMovie>(jsonResponse);
                    if (!jsonMovie.available)
                        return null;

                    var convertedMovie = Convert(jsonMovie);
                    return convertedMovie?.TheMovieDbId == theMovieDbId.ToString() ? convertedMovie : null;
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while searching for a movie by tmdbId \"{theMovieDbId}\" from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while searching for a movie by tmdbId from Ombi");
        }



        /// <summary>
        /// Handles the submission of issues to Ombi
        /// </summary>
        /// <param name="request"></param>
        /// <param name="theMovieDbId"></param>
        /// <param name="issueValue"></param>
        /// <param name="issueDescription"></param>
        /// <returns>True if succesfull, false if failed to submit</returns>
        /// <exception cref="System.Exception">Something went wrong contacting Ombi</exception>
        public async Task<bool> SubmitMovieIssueAsync(MovieRequest request, int theMovieDbId, string issueValue, string issueDescription)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var response = await HttpGetAsync($"{BaseURL}/api/v1/search/movie/info/{theMovieDbId}");
                    await response.ThrowIfNotSuccessfulAsync("OmbiMovieSearchByMovieDbId failed", x => x.error);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonMovie = JsonConvert.DeserializeObject<JSONMovie>(jsonResponse);
                    if (!jsonMovie.available)
                        return false;

                    var ombiUser = await FindLinkedOmbiUserAsync(request.User.UserId, request.User.Username);
                    response = await HttpPostAsync(ombiUser, $"{BaseURL}/api/v1/Issues", JsonConvert.SerializeObject(new
                    {
                        title = jsonMovie.title,
                        requestType = jsonMovie.type,
                        providerId = string.IsNullOrWhiteSpace(jsonMovie.imdbId) ? jsonMovie.theMovieDbId : jsonMovie.imdbId,
                        subject = IssueTypes.Where(x => x.Value.ToString() == issueValue).FirstOrDefault().Key,
                        description = issueDescription,
                        issueCategoryId = int.Parse(issueValue)
                    }));

                    await response.ThrowIfNotSuccessfulAsync("OmbiIssueMovieRequest failed", x => x.error);
                    return true;
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while searching for movies from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while searching for movies from Ombi");
        }



        /// <summary>
        /// Handles the searching for TV Shows with the name in your own library
        /// </summary>
        /// <param name="request"></param>
        /// <param name="tvShowName"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<IReadOnlyList<SearchedTvShow>> SearchTvShowLibraryAsync(TvShowRequest request, string tvShowName)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var response = await HttpGetAsync($"{BaseURL}/api/v1/Search/Tv/{tvShowName}");

                    await response.ThrowIfNotSuccessfulAsync("OmbiSearchTvShow failed", x => x.error);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonTvShows = JsonConvert.DeserializeObject<List<JSONTvShow>>(jsonResponse)
                        .Where(x => x.available)
                        .ToArray();

                    return jsonTvShows.Select(x => new SearchedTvShow
                    {
                        Title = x.title,
                        Banner = x.banner,
                        TheTvDbId = x.id,
                        FirstAired = x.firstAired,
                    }).ToArray();
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while searching for tv show \"{tvShowName}\" from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while searching for tv show from Ombi");
        }



        /// <summary>
        /// This handles the searching for a TV show with the TVDB Id in your own library
        /// </summary>
        /// <param name="request"></param>
        /// <param name="tvDbId"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<SearchedTvShow> SearchTvShowLibraryAsync(TvShowRequest request, int tvDbId)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var response = await HttpGetAsync($"{BaseURL}/api/v1/Search/Tv/info/{tvDbId}");
                    await response.ThrowIfNotSuccessfulAsync("OmbiSearchTvShowByTvDbId failed", x => x.error);

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonTvShow = JsonConvert.DeserializeObject<JSONTvShow>(jsonResponse);
                    if (!jsonTvShow.available)
                        return null;

                    var searchedTvShow = new SearchedTvShow
                    {
                        Title = jsonTvShow.title,
                        Banner = jsonTvShow.banner,
                        TheTvDbId = jsonTvShow.id,
                        FirstAired = jsonTvShow.firstAired,
                    };

                    return searchedTvShow.TheTvDbId == tvDbId ? searchedTvShow : null;
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while searching for tv show by tvDbId \"{tvDbId}\" from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("An error occurred while searching for tv show by tvDbId from Ombi");
        }



        /// <summary>
        /// Handles the submission of the issue to Ombi
        /// </summary>
        /// <param name="request"></param>
        /// <param name="theTvDbId"></param>
        /// <param name="issueValue"></param>
        /// <param name="issueDescription"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async Task<bool> SubmitTvShowIssueAsync(TvShowRequest request, int theTvDbId, string issueValue, string issueDescription)
        {
            var retryCount = 0;

            while (retryCount <= 5)
            {
                try
                {
                    var jsonTvShow = await FindTvShowByTheTvDbIdAsync(theTvDbId.ToString());
                    if (!jsonTvShow.available)
                        return false;


                    var ombiUser = await FindLinkedOmbiUserAsync(request.User.UserId, request.User.Username);

                    var response = await HttpPostAsync(ombiUser, $"{BaseURL}/api/v1/Issues", JsonConvert.SerializeObject(new
                    {
                        title = jsonTvShow.title,
                        requestType = jsonTvShow.type,
                        providerId = jsonTvShow.id.ToString(),
                        subject = IssueTypes.Where(x => x.Value.ToString() == issueValue).FirstOrDefault().Key,
                        description = issueDescription,
                        issueCategoryId = int.Parse(issueValue)
                    }));

                    await response.ThrowIfNotSuccessfulAsync("OmbiIssueTvShowRequest failed", x => x.error);
                    return true;
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while searching for movies from Ombi: " + ex.Message);
                    retryCount++;
                    await Task.Delay(1000);
                }
            }

            throw new System.Exception("Something went wrong while handling the submission of an tv show issue from Ombi");
        }


        /// <summary>
        /// Used to hold categorie information from Ombi
        /// </summary>
        public class JSONIssueCategorie
        {
            [JsonProperty("value")]
            public string CategorieName { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }
        }


        public class JSONMovie
        {
            public string title { get; set; }
            public bool available { get; set; }
            public string quality { get; set; }
            public bool requested { get; set; }
            public bool approved { get; set; }
            public string plexUrl { get; set; }
            public string embyUrl { get; set; }
            public string overview { get; set; }
            public string posterPath { get; set; }
            public string releaseDate { get; set; }
            public string theMovieDbId { get; set; }
            public int type { get; set; } = 1;
            public string imdbId { get; set; } = string.Empty;
        }

        public class JSONTvShow
        {
            public int id { get; set; }

            public string imdbId { get; set; } = string.Empty;
            public int type { get; set; } = 0;
            public string title { get; set; }
            public string quality { get; set; }
            public string plexUrl { get; set; }
            public string embyUrl { get; set; }
            public string overview { get; set; }
            public bool requested { get; set; }
            public bool available { get; set; }
            public string banner { get; set; }
            public string firstAired { get; set; }
            public string network { get; set; }
            public string status { get; set; }
            public JSONTvSeason[] seasonRequests { get; set; }
        }

        public class JSONTvSeason
        {
            public int seasonNumber { get; set; }
            public JSONTvEpisode[] episodes { get; set; }

            public bool CanBeRequested()
            {
                return episodes.Any(e => e.CanBeRequested());
            }
        }

        public class JSONTvEpisode
        {
            public int episodeNumber { get; set; }
            public bool available { get; set; }
            public bool requested { get; set; }

            public bool CanBeRequested()
            {
                return !available && !requested;
            }
        }

        public class JSONClaim
        {
            public string value { get; set; }
            public bool enabled { get; set; }
        }

        public class JSONRequestQuota
        {
            public bool hasLimit { get; set; }
            public int remaining { get; set; }
        }

        public class JSONOmbiUser
        {
            public string id { get; set; }
            public string userName { get; set; }
            public string alias { get; set; }
            public List<JSONClaim> claims { get; set; }
            public JSONRequestQuota episodeRequestQuota { get; set; }
            public JSONRequestQuota movieRequestQuota { get; set; }
            public bool IsAdmin => claims?.Any(x => x.value.Equals("admin", StringComparison.InvariantCultureIgnoreCase) && x.enabled) ?? false;
            public bool IsDisabled => claims?.Any(x => x.value.Equals("disabled", StringComparison.InvariantCultureIgnoreCase) && x.enabled) ?? false;
            public bool AutoApproveTv => claims?.Any(x => x.value.Equals("autoapprovetv", StringComparison.InvariantCultureIgnoreCase) && x.enabled) ?? false;
            public bool AutoApproveMovies => claims?.Any(x => x.value.Equals("autoapprovemovie", StringComparison.InvariantCultureIgnoreCase) && x.enabled) ?? false;
            public bool CanRequestTv => !IsDisabled && (IsAdmin || AutoApproveTv || (claims?.Any(x => x.value.Equals("requesttv", StringComparison.InvariantCultureIgnoreCase) && x.enabled) ?? false));
            public bool CanRequestMovie => !IsDisabled && (IsAdmin || AutoApproveMovies || (claims?.Any(x => x.value.Equals("requestmovie", StringComparison.InvariantCultureIgnoreCase) && x.enabled) ?? false));
        }

        public class OmbiUser
        {
            public string Username { get; set; }
            public string ApiAlias { get; set; }
            public bool CanRequestTvShows { get; set; }
            public int TvEpisodeQuotaRemaining { get; set; }
            public bool CanRequestMovies { get; set; }
            public int MoviesQuotaRemaining { get; set; }
        }
    }
}
