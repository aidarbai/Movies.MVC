using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cinema.BLL.DTOs.Movie;
using Cinema.DAL.DbContext;
using Cinema.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinema.BLL.Helpers.YouTubeLink;
using Microsoft.AspNetCore.Identity;
using Cinema.COMMON.Filters;
using Cinema.COMMON.Responses;
using Cinema.BLL.Helpers.Pagination;

namespace Cinema.BLL.Services.Movies
{
    public class MovieService : BaseService, IMovieService
    {
        private readonly CinemaDbContext _db;
        private readonly IMovieContentHelper _movieContentHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public MovieService(
            CinemaDbContext db,
            IMovieContentHelper movieContentHelper,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _movieContentHelper = movieContentHelper;
            _userManager = userManager;
        }

        #region CRUD
        public async Task<BaseResponse> CreateMovieAsync(
            MovieDTO dto)
        {
            BaseResponse response = new();

            try
            {
                var movie = Mapper.Map<Movie>(dto);
                movie.CreationDate = DateTime.Now;

                _db.Movies.Add(movie);
                await _db.SaveChangesAsync();

                response.Message = "Movie has been created successfully";
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = "Error while creating a new movie";
                response.Ex = ex;
            }

            return response;
        }

        public PaginatedResponse<MovieDTO> GetAllMoviesPaginated(
            ViewFilter filter)
        {
            var movies = _db.Movies.ProjectTo<MovieDTO>().OrderByDescending(m => m.Vote_average).AsQueryable();

            var response = PaginationHelper.GetPaginatedList(filter, movies);

            return response;
        }

        public PaginatedResponse<MovieDTO> GetPaginatedMoviesForAdmin(
            ViewFilter filter)
        {
            PaginatedResponse<MovieDTO> response = new();

            try
            {
                var moviesQuery = _db.Movies.ProjectTo<MovieDTO>().AsQueryable();

                if (!string.IsNullOrEmpty(filter.SearchString))
                {
                    moviesQuery = moviesQuery.Where(m => m.Title.Contains(filter.SearchString)
                                           || m.Overview.Contains(filter.SearchString));
                }

                if (!moviesQuery.Any())
                {
                    response.Succeeded = false;
                    return response;
                }

                switch (filter.SortOrder)
                {
                    case "title_desc":
                        moviesQuery = moviesQuery.OrderByDescending(m => m.Title);
                        break;
                    default:
                        moviesQuery = moviesQuery.OrderBy(m => m.Title);
                        break;
                }

                response = PaginationHelper.GetPaginatedList(filter, moviesQuery);
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
            }

            return response;
        }

        public async Task<ResponseWithPayload<MovieDTO>> GetMovieAsync(
            int id)
        {
            ResponseWithPayload<MovieDTO> response = new();
            try
            {
                var movie = await _db.Movies.Where(m => m.Id == id).ProjectTo<MovieDTO>().AsSingleQuery().FirstOrDefaultAsync();
                response.Data = movie;
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
            }

            return response;

        }
        public PaginatedResponse<MovieDTO> GetMovieByGenre(
            string genre,
            ViewFilter filter)
        {
            var movies = _db.Movies.Where(m => m.Genres.Any(g => g.Name == genre)).ProjectTo<MovieDTO>().AsQueryable();
            return PaginationHelper.GetPaginatedList(filter, movies);
        }

        public async Task<ResponseWithPayload<List<MovieDTO>>> GetSearchAsync(
            string searchString)
        {
            ResponseWithPayload<List<MovieDTO>> response = new();
            try
            {
                var searchResult = await _db.Movies.Where(m => m.Title.Contains(searchString) || m.Overview.Contains(searchString)).ProjectTo<MovieDTO>().AsSingleQuery().ToListAsync();
                response.Data = searchResult;
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
            }
            return response;
        }

        public async Task<BaseResponse> UpdateMovieAsync(
            MovieDTO dto)
        {
            BaseResponse response = new();

            try
            {
                var requestedMovie = _db.Movies.FirstOrDefault(m => m.Id == dto.Id);
                if (requestedMovie != null)
                {
                    requestedMovie.Overview = dto.Overview;
                    requestedMovie.Title = dto.Title;
                    requestedMovie.Youtube_path = dto.Youtube_path;
                    _db.Movies.Update(requestedMovie);
                    await _db.SaveChangesAsync();

                    response.Message = "Movie has been updated successfully";
                }
                else
                {
                    response.Message = "Movie not found";
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
                response.Message = "Error while updating the movie";
            }

            return response;
        }

        public async Task<BaseResponse> VoteAsync(
            int id,
            string userId,
            bool upVote)
        {
            BaseResponse response = new();
            try
            {
                var movie = await _db.Movies.Where(m => m.Id == id).FirstOrDefaultAsync();
                var loggedUser = await _userManager.FindByIdAsync(userId);


                if (movie != null)
                {
                    if (movie.Votes.Any(v => v.User.Id == loggedUser.Id))
                    {
                        response.Message = "You have voted already. The user can vote once.";
                        return response;
                    }
                    movie.Votes.Add(new Vote { IsUpVote = upVote, User = loggedUser });
                    await _db.SaveChangesAsync();
                    response.Message = "Your vote has been added successfully";
                }
                else
                {
                    response.Message = "Movie not found";
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
                response.Message = "An error occurred while adding vote to the movie.";
            }

            return response;
        }
        public async Task<BaseResponse> AddCommentAsync(
            int id,
            string userId,
            string comment)
        {
            BaseResponse response = new();

            try
            {
                var movie = await _db.Movies.Where(m => m.Id == id).FirstOrDefaultAsync();
                var loggedUser = await _userManager.FindByIdAsync(userId);
                if (movie != null)
                {
                    movie.Comments.Add(new Comment { Text = comment, User = loggedUser });
                    await _db.SaveChangesAsync();
                    response.Message = "Comment has been added successfully";
                }
                else
                {
                    response.Message = "Movie not found";
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
                response.Message = "An error occurred while adding comment to the movie.";
            }

            return response;
        }

        public async Task<BaseResponse> EditCommentAsync(
            int id,
            string userId,
            int commentId,
            string comment)
        {
            BaseResponse response = new();

            try
            {
                var movie = await _db.Movies.Where(m => m.Id == id).FirstOrDefaultAsync();
                var commentToEdit = movie.Comments.Where(c => c.Id == commentId && c.User.Id == userId).FirstOrDefault();
                var loggedUser = await _userManager.FindByIdAsync(userId);
                if (movie != null && commentToEdit != null)
                {
                    commentToEdit.Text = comment;
                    await _db.SaveChangesAsync();
                    response.Message = "Comment has been updated successfully";
                }
                else
                {
                    response.Message = "Movie and comment not found";
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
                response.Message = "An error occurred while adding comment to the movie.";
            }

            return response;
        }

        public async Task<BaseResponse> DeleteMovieAsync(int id)
        {
            BaseResponse response = new();

            try
            {
                var requestedMovie = await _db.Movies.FindAsync(id);
                if (requestedMovie != null)
                {
                    requestedMovie.IsDeleted = true;
                    _db.Movies.Update(requestedMovie);
                    await _db.SaveChangesAsync();
                    response.Message = "Movie has been deleted successfully";
                }
                else
                {
                    response.Message = "Movie not found";
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
                response.Message = "Error while deleting the movie";
            }

            return response;
        }

        public async Task<BaseResponse> BulkDeleteMovie(int[] moviesArray)
        {
            BaseResponse response = new();

            response.Message = "Movies have been deleted successfully";

            foreach (var item in moviesArray)
            {
                var result = await DeleteMovieAsync(item);
                if (!result.Succeeded)
                {
                    response.Succeeded = false;
                    response.Ex = result.Ex;
                    response.Message = result.Message;
                    break;
                }
            }

            return response;
        }

        #endregion

        public async Task<ResponseWithCount> UploadMoviesAsync()
        {
            ResponseWithCount response = new();

            try
            {
                var moviesList = await _movieContentHelper.GetMovieList();
                await _db.Movies.AddRangeAsync(moviesList);
                await _db.SaveChangesAsync();
                response.TotalCount = moviesList.Count;
                response.Message = $"{moviesList.Count} movies have been uploaded";
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
                response.Message = "Error while uploading new movies";
            }

            return response;
        }

        public ResponseWithPayload<MovieDTO[]> GetTrailers()
        {
            var response = new ResponseWithPayload<MovieDTO[]>();

            try
            {
                var allMovies = _db.Movies.ProjectTo<MovieDTO>().OrderByDescending(m => m.CreationDate).AsSplitQuery().AsQueryable();
                var trailers = allMovies.Where(m => !string.IsNullOrEmpty(m.Youtube_path)).AsSingleQuery().Take(10).ToArray();
                response.Data = trailers;
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
            }

            return response;
        }
    }
}
