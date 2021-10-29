using Cinema.BLL.DTOs.Movie;
using Cinema.COMMON.Filters;
using Cinema.COMMON.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema.BLL.Services.Movies
{
    public interface IMovieService
    {
        Task<BaseResponse> AddCommentAsync(int id, string userId, string comment);
        Task<BaseResponse> BulkDeleteMovie(int[] moviesArray);
        Task<BaseResponse> CreateMovieAsync(MovieDTO dto);
        Task<BaseResponse> DeleteMovieAsync(int id);
        Task<BaseResponse> EditCommentAsync(int id, string userId, int commentId, string comment);
        PaginatedResponse<MovieDTO> GetAllMoviesPaginated(ViewFilter filter);
        Task<ResponseWithPayload<MovieDTO>> GetMovieAsync(int id);
        PaginatedResponse<MovieDTO> GetMovieByGenre(string genre, ViewFilter filter);
        PaginatedResponse<MovieDTO> GetPaginatedMoviesForAdmin(ViewFilter filter);
        Task<ResponseWithPayload<List<MovieDTO>>> GetSearchAsync(string searchString);
        ResponseWithPayload<MovieDTO[]> GetTrailers();
        Task<BaseResponse> UpdateMovieAsync(MovieDTO dto);
        Task<ResponseWithCount> UploadMoviesAsync();
        Task<BaseResponse> VoteAsync(int id, string userId, bool upVote);
    }
}