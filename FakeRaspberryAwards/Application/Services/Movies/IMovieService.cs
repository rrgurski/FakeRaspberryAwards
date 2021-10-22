namespace FakeRaspberryAwards.Application.Services.Movies
{
    public interface IMovieService
    {
        void ImportFromCsv(string csv);
        AwardsIntervalResult GetAwardsInterval();
    }
}
