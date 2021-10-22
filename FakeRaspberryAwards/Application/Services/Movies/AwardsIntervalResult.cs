using System.Collections.Generic;

namespace FakeRaspberryAwards.Application.Services.Movies
{
    public class AwardsIntervalResult
    {
        public AwardsIntervalResult(IEnumerable<AwardsInterval> min, IEnumerable<AwardsInterval> max)
        {
            Min = min;
            Max = max;
        }

        public IEnumerable<AwardsInterval> Min { get; }
        public IEnumerable<AwardsInterval> Max { get; }

        public class AwardsInterval
        {
            public AwardsInterval(string producer, int interval, int previousWin, int followingWin)
            {
                Producer = producer;
                Interval = interval;
                PreviousWin = previousWin;
                FollowingWin = followingWin;
            }

            public string Producer { get; }
            public int Interval { get; }
            public int PreviousWin { get; }
            public int FollowingWin { get; }
        }
    }
}
