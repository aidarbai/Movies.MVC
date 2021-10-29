using Cinema.BLL.Services.Movies;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace Cinema.BLL.Jobs
{
    [DisallowConcurrentExecution]
    public class UploadNewMoviesJob : IJob
    {
        private readonly ILogger<UploadNewMoviesJob> _logger;
        private readonly IMovieService _movieService;
        public UploadNewMoviesJob(ILogger<UploadNewMoviesJob> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("UploadNewMoviesJob run");
            var response = await _movieService.UploadMoviesAsync();
            if (!response.Succeeded && response.Ex != null)
            {
                _logger.LogError(response.Ex.Message, response.Ex);
            }
            _logger.LogInformation(response.Message);
        }
    }
}