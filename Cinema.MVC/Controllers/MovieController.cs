using Cinema.BLL.DTOs.Movie;
using Cinema.BLL.Services.Movies;
using Cinema.COMMON.Constants;
using Cinema.COMMON.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Cinema.MVC.Controllers
{
    [Authorize(Roles = AppConstants.Roles.Groups.ADMINSGROUP)]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MovieController> _logger;
        
        public MovieController(
            ILogger<MovieController> logger,
            IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }
        
        public IActionResult Index(ViewFilter filter)
        {
            filter.PageSize = filter.PageSize <= 1 ? 5 : filter.PageSize;
            filter.SortOrder = string.IsNullOrEmpty(filter.SortOrder) ? "title" : filter.SortOrder;

            ViewData["CurrentSort"] = filter.SortOrder;
            ViewData["TitleSortParm"] = filter.SortOrder == "title" ? "title_desc" : "title";

            if (filter.SearchString != null)
            {
                filter.PageNumber = 1;
            }
            else
            {
                filter.SearchString = filter.CurrentFilter;
            }

            ViewData["CurrentFilter"] = filter.SearchString;

            var response = _movieService.GetPaginatedMoviesForAdmin(filter);

            if (response.Succeeded)
            {
                ViewBag.Movies = response.Items;
                ViewBag.TotalPages = response.PagesCount;
                ViewBag.PageNumber = response.CurrentPage;
            }
            else if (response.Ex != null)
            {
                _logger.LogError(response.Ex.Message, response.Ex);
            }

            ViewBag.PageSize = filter.PageSize;

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UploadNewMoviesAsync()
        {
            var response = await _movieService.UploadMoviesAsync();
            if (!response.Succeeded && response.Ex != null)
            {
                _logger.LogError(response.Ex.Message, response.Ex);
            }

            TempData["Message"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(MovieDTO dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Incorrect input";
                return View();
            }

            var response = await _movieService.CreateMovieAsync(dto);
            if (!response.Succeeded && response.Ex != null)
            {
                _logger.LogError(response.Ex.Message, response.Ex);
            }

            TempData["Message"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(MovieDTO dto)
        {
            if (dto.Id <= 0)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Incorrect input";
                return RedirectToAction(nameof(Index));
            }

            var response = await _movieService.UpdateMovieAsync(dto);
            if (!response.Succeeded && response.Ex != null)
            {
                _logger.LogError(response.Ex.Message, response.Ex);
            }

            TempData["Message"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var response = await _movieService.DeleteMovieAsync(id);
            if (!response.Succeeded && response.Ex != null)
            {
                _logger.LogError(response.Ex.Message, response.Ex);
            }

            TempData["Message"] = response.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDeleteAsync(IFormCollection id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var values = id["articlesArray"].ToString();
            string[] str = values.Split(new char[] { ',' });

            var response = await _movieService.BulkDeleteMovie(Array.ConvertAll(str, s => int.Parse(s)));
            if (!response.Succeeded && response.Ex != null)
            {
                _logger.LogError(response.Ex.Message, response.Ex);
            }

            TempData["Message"] = response.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
