using Cinema.BLL.DTOs.Movie;
using Cinema.BLL.Helpers.Url;
using Cinema.BLL.Services.Storage;
using Cinema.COMMON.Responses;
using Cinema.DAL.DbContext;
using Cinema.DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cinema.BLL.Helpers.YouTubeLink
{
    public class MovieContentHelper : IMovieContentHelper
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MovieContentHelper> _logger;
        private readonly CinemaDbContext _db;
        private readonly IGoogleCloudService _uploader;

        private readonly string moviePath;

        public MovieContentHelper(IConfiguration configuration, ILogger<MovieContentHelper> logger, CinemaDbContext db, IGoogleCloudService uploader, HttpClient client)
        {
            _configuration = configuration;
            _logger = logger;
            _db = db;
            _uploader = uploader;
            _client = client;
            moviePath = _configuration["MovieDB:Domain"] + _configuration["MovieDB:MovieLink"];
        }
        private async Task<T> ParseJson<T>(string url) where T : class
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            return JsonConvert.DeserializeObject<T>(responseBody, settings);
        }

        private async Task<Movie> GetYoutubeLink(Movie movie)
        {
            try
            {
                string queryStr = moviePath + movie.ExternalId + _configuration["MovieDB:MovieVideos"] + _configuration["MovieDB:ApiKeyAsParameter"];
                var jsonModel = await ParseJson<JsonResultsMovieYoutube>(queryStr);
                List<MovieYoutubeInfo> movieList = jsonModel.Results;
                if (movieList.Count > 0)
                {
                    string link = movieList.Where(
                                    m => m.Site == "YouTube" &&
                                    m.Type == "Trailer")
                                    .OrderByDescending(m => m.Published_at)
                                    .FirstOrDefault().Key;
                    if (!string.IsNullOrEmpty(link))
                    {
                        movie.Youtube_path = _configuration["YouTubeEmbedLink"] + link;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

            return movie;
        }
        private async Task<Movie> GetGenresAndHomepage(Movie movie)
        {
            try
            {
                string queryStr = moviePath + movie.ExternalId + _configuration["MovieDB:ApiKeyAsParameter"];

                var downloadedMovie = await ParseJson<Movie>(queryStr);

                if (downloadedMovie != null)
                {
                    movie.Homepage = downloadedMovie.Homepage;
                    var genres = downloadedMovie.Genres;
                    foreach (var item in genres)
                    {
                        var genre = _db.Genres.Where(g => g.ExternalId == item.ExternalId).FirstOrDefault();
                        if (genre == null)
                        {
                            var newGenre = new Genres
                            {
                                ExternalId = item.ExternalId,
                                Name = item.Name
                            };
                            _db.Genres.Add(newGenre);
                            movie.Genres.Add(newGenre);
                        }
                        else
                        {
                            movie.Genres.Add(genre);
                        }
                    }

                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
            return movie;
        }

        private async Task<Movie> GetAndUploadPictures(Movie movie)
        {
            BaseResponse response = new();
            try
            {
                if (!string.IsNullOrEmpty(movie.Poster_path))
                {
                    string origPosterSmallPath = _configuration["MovieDB:ImagePath"] + _configuration["MovieDB:SmallPostersPath"] + movie.Poster_path;
                    var fileNameSmallPoster = FileNameHelper.GetFileNameFromUrl(origPosterSmallPath);
                    response = await _uploader.UploadFileAsync(origPosterSmallPath, fileNameSmallPoster);
                    if (!response.Succeeded && response.Ex != null)
                    {
                        _logger.LogError(response.Ex.Message, response.Ex);
                    }
                    movie.Poster_small_path = response.Message;

                    string origHorizontalSmallImagePath = _configuration["MovieDB:ImagePath"] + _configuration["MovieDB:HorizontalSmallImagePath"] + movie.Poster_path;
                    var fileNameHorizontalSmallImage = FileNameHelper.GetFileNameFromUrl(origHorizontalSmallImagePath);
                    response = await _uploader.UploadFileAsync(origHorizontalSmallImagePath, fileNameHorizontalSmallImage);
                    if (!response.Succeeded && response.Ex != null)
                    {
                        _logger.LogError(response.Ex.Message, response.Ex);
                    }
                    movie.Horizontal_small_image_path = response.Message;
                }
                if (!string.IsNullOrEmpty(movie.Backdrop_path))
                {
                    string origBackdropImagePath = _configuration["MovieDB:ImagePath"] + _configuration["MovieDB:PostersPath"] + movie.Backdrop_path;
                    var fileNameBackdrop = FileNameHelper.GetFileNameFromUrl(origBackdropImagePath);
                    response = await _uploader.UploadFileAsync(origBackdropImagePath, fileNameBackdrop);
                    if (!response.Succeeded && response.Ex != null)
                    {
                        _logger.LogError(response.Ex.Message, response.Ex);
                    }
                    movie.Backdrop_path = response.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

            return movie;
        }
        private async Task<List<Movie>> GetMovies()
        {
            List<Movie> movieList = new();
            try
            {
                string queryStr = moviePath + _configuration["MovieDB:LatestMovies"] + _configuration["MovieDB:ApiKeyAsParameter"];
                var jsonModel = await ParseJson<JsonResultsMovie>(queryStr);
                movieList = jsonModel.Results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

            return movieList;
        }
        public async Task<List<Movie>> GetMovieList()
        {
            List<Movie> downloadedMovies = (await GetMovies()).Where(d => !_db.Movies.Any(m => m.ExternalId == d.ExternalId)).ToList();

            for (int i = 0; i < downloadedMovies.Count; i++)
            {
                downloadedMovies[i] = await GetYoutubeLink(downloadedMovies[i]);
                downloadedMovies[i] = await GetGenresAndHomepage(downloadedMovies[i]);
                downloadedMovies[i] = await GetAndUploadPictures(downloadedMovies[i]);
                downloadedMovies[i].CreationDate = DateTime.Now;
            }

            return downloadedMovies;
        }
    }
}
