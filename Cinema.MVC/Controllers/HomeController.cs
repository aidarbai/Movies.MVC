using Cinema.BLL.Services.Movies;
using Cinema.COMMON.Filters;
using Cinema.DAL.Models;
using Cinema.MVC.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cinema.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, IMovieService movieService, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _movieService = movieService;
            _userManager = userManager;

        }
        public IActionResult Index(ViewFilter filter)
        {
            filter.PageSize = filter.PageSize <= 1 ? 20 : filter.PageSize;

            var responseForTrailers = _movieService.GetTrailers();
            if (responseForTrailers.Succeeded)
            {
                ViewBag.Trailers = responseForTrailers.Data;
            }
            else
            {
                _logger.LogError(responseForTrailers.Ex.Message, responseForTrailers.Ex);
            }

            var responseForMovies = _movieService.GetAllMoviesPaginated(filter);

            if (responseForMovies.Succeeded)
            {
                ViewBag.PageNumber = responseForMovies.CurrentPage;
                ViewBag.TotalPages = responseForMovies.PagesCount;
            }
            else
            {
                _logger.LogError(responseForMovies.Ex.Message, responseForMovies.Ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (responseForMovies.Items == null || responseForMovies.Items.Count == 0)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return View(responseForMovies.Items);
        }
        public IActionResult Genre(string id, ViewFilter filter)
        {
            if (id == null)
            {
                return NotFound();
            }

            filter.PageSize = filter.PageSize <= 1 ? 4 : filter.PageSize;
            
            var response = _movieService.GetMovieByGenre(id, filter);

            if (!response.Succeeded)
            {
                _logger.LogError(response.Ex.Message, response.Ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (response.Items == null || response.Items.Count == 0)
            {
                return NotFound();
            }

            ViewBag.Genre = id;

            ViewBag.PageNumber = response.CurrentPage;
            ViewBag.TotalPages = response.PagesCount;

            return View(response.Items);
        }


        [HttpGet("/film/{id}")]
        public async Task<IActionResult> GetMovieByIdAsync(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var requestedMovie = await _movieService.GetMovieAsync(id.Value);

            if (!requestedMovie.Succeeded || requestedMovie.Data == null)
            {
                if (requestedMovie.Ex != null)
                {
                    _logger.LogError(requestedMovie.Ex.Message, requestedMovie.Ex);
                }
                return NotFound();
            }

            return View(requestedMovie.Data);
        }
        [HttpGet("/Home/GetAjaxSearchMovie/{searchString}")]
        public async Task<IActionResult> GetAjaxSearchMovieAsync(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return NotFound(new JsonResult(new { }));
            }

            var results = await _movieService.GetSearchAsync(searchString);

            if (!results.Succeeded || results.Data == null || results.Data.Count == 0)
            {
                if (results.Ex != null)
                {
                    _logger.LogError(results.Ex.Message, results.Ex);
                }
                return NotFound(new JsonResult(new { }));
            }

            return Ok(results.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VoteAsync(int? id, bool? upVote)
        {
            if (id == null || upVote == null)
            {
                return NotFound();
            }

            var loggedUser = await _userManager.GetUserAsync(User);

            string result;

            if (loggedUser == null)
            {
                result = "Please login if you'd like to vote.";
            }
            else
            {
                var response = await _movieService.VoteAsync(id.Value, loggedUser.Id, upVote.Value);
                
                if (!response.Succeeded)
                {
                    _logger.LogError(response.Ex.Message, response.Ex);
                }
                
                result = response.Message;
            }

            TempData["Message"] = result;
            return RedirectToAction("GetMovieById", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCommentAsync(int? id, string comment)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(comment))
            {
                ViewBag.Message = "Can't add empty comment";
                return View();
            }
            var loggedUser = await _userManager.GetUserAsync(User);

            string result;

            if (loggedUser == null)
            {
                result = "Please login if you'd like to post comment.";
            }
            else
            {
                var response = await _movieService.AddCommentAsync(id.Value, loggedUser.Id, comment);
                
                if (!response.Succeeded)
                {
                    _logger.LogError(response.Ex.Message, response.Ex);
                }
                
                result = response.Message;
            }

            TempData["Message"] = result;
            return RedirectToAction("GetMovieById", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCommentAsync(int? id, int? commentId, string comment)
        {
            if (id == null || commentId == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(comment))
            {
                ViewBag.Message = "Can't add empty comment";
                return View();
            }

            string result;
            var loggedUser = await _userManager.GetUserAsync(User);
            if (loggedUser == null)
            {
                result = "Please login if you'd like to post comment.";
            }
            else
            {
                var response = await _movieService.EditCommentAsync(id.Value, loggedUser.Id, commentId.Value, comment);

                if (!response.Succeeded)
                {
                    _logger.LogError(response.Ex.Message, response.Ex);
                }

                result = response.Message;
            }

            TempData["Message"] = result;
            return RedirectToAction("GetMovieById", new { id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionObject = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (exceptionObject != null)
            {
                string errorMessage = exceptionObject.Error.Message + " TraceId: " + Activity.Current?.Id ?? HttpContext.TraceIdentifier;
                _logger.LogError(errorMessage, exceptionObject.Error);
            }

            return View(new ErrorViewModel { ErrorCode = HttpContext.Response.StatusCode });
        }
    }
}
