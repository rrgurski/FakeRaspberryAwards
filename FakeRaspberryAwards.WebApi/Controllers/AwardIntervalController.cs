using FakeRaspberryAwards.Application.Services.Movies;
using Microsoft.AspNetCore.Mvc;

namespace FakeRaspberryAwards.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AwardIntervalController : ControllerBase
    {
        public AwardIntervalController(IMovieService movieService)
        {
            MovieService = movieService;
        }

        private IMovieService MovieService { get; }

        [HttpGet]
        public AwardsIntervalResult Get()
        {
            var result = MovieService.GetAwardsInterval();
            return result;
        }
    }
}
