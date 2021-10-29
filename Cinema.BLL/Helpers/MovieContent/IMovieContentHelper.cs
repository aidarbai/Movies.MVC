using Cinema.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema.BLL.Helpers.YouTubeLink
{
    public interface IMovieContentHelper
    {
        Task<List<Movie>> GetMovieList();
    }
}