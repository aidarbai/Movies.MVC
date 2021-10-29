using Cinema.DAL.DbContext;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Cinema.BLL.Helpers.Genre
{
    public class GenresHelper : IGenresHelper
    {
        private readonly CinemaDbContext _db;
        private readonly ILogger<GenresHelper> _logger;
        public GenresHelper(CinemaDbContext db, ILogger<GenresHelper> logger)
        {
            _db = db;
            _logger = logger;
        }

        public string[] GetAllGenres()
        {
            try
            {
                var genres = _db.Genres.Select(g => g.Name).OrderBy(g => g).ToArray();
                return genres;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new string[] { };
            }
        }
    }
}
