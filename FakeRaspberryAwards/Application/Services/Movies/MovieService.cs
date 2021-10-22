using FakeRaspberryAwards.Domain.Entities;
using FakeRaspberryAwards.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FakeRaspberryAwards.Application.Services.Movies
{
    public class MovieService : IMovieService
    {
        public MovieService(DatabaseContext databaseContext)
        {
            DatabaseContext = databaseContext;
        }

        private DatabaseContext DatabaseContext { get; }

        public void ImportFromCsv(string csv)
        {
            var movies = new List<Movie>();
            var studios = new List<Studio>();
            var producers = new List<Producer>();

            var lineCount = 1;
            var lines = csv.Split("\n").Skip(1);
            foreach (var line in lines)
            {
                lineCount++;

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var fields = line.Split(";");
                if (fields.Length < 4)
                {
                    throw new ArgumentException($"Invalid column count (line {lineCount}).", nameof(csv));
                }

                if (!int.TryParse(fields[0], out int year))
                {
                    throw new ArgumentException($"Column 'year' has an invalid value (line {lineCount}).", nameof(csv));
                }

                var movie = new Movie
                {
                    Year = year,
                    Title = fields[1],
                    Winner = fields.Length > 4 && fields[4].Equals("yes"),
                };

                movies.Add(movie);

                var studiosNames = fields[2].Split(new string[] { ", and ", " and ", ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var studioName in studiosNames)
                {
                    var studio = studios.FirstOrDefault(o => o.Name == studioName);
                    if (studio == null)
                    {
                        studio = new Studio { Name = studioName };
                        studios.Add(studio);
                    }
                    movie.Studios.Add(studio);
                }

                var producersNames = fields[3].Split(new string[] { ", and ", " and ", ", " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var producerName in producersNames)
                {
                    var producer = producers.FirstOrDefault(o => o.Name == producerName);
                    if (producer == null)
                    {
                        producer = new Producer { Name = producerName };
                        producers.Add(producer);
                    }
                    movie.Producers.Add(producer);
                }
            }

            DatabaseContext.Set<Movie>().AddRange(movies);
            DatabaseContext.SaveChanges();
        }

        public AwardsIntervalResult GetAwardsInterval()
        {
            var min = new List<AwardsIntervalResult.AwardsInterval>();
            var max = new List<AwardsIntervalResult.AwardsInterval>();

            var producers = DatabaseContext.Set<Producer>().Include(o => o.Movies).ToList();
            foreach (var producer in producers)
            {
                // winner movies, ordered by year
                var winners = producer.Movies
                    .Where(o => o.Winner)
                    .OrderBy(o => o.Year)
                    .ToList();

                // only those who won more than once
                if (winners.Count < 2)
                {
                    continue;
                }

                // iterate over all but the last in each group
                for (var i = 0; i < (winners.Count - 1); i++)
                {
                    var winner1 = winners[i];
                    var winner2 = winners[i + 1];
                    var interval = winner2.Year - winner1.Year;

                    // current minimum interval is equal to (tie)
                    if (!min.Any() || min[0].Interval == interval)
                    {
                        AddInterval(min, producer.Name, interval, winner1, winner2);
                    }
                    // current minimum interval is greater than (new minimum interval)
                    else if (min[0].Interval > interval)
                    {
                        // clear the list before adding the new one
                        min.Clear();
                        AddInterval(min, producer.Name, interval, winner1, winner2);
                    }

                    // current maximum interval is equal to (tie)
                    if (!max.Any() || max[0].Interval == interval)
                    {
                        AddInterval(max, producer.Name, interval, winner1, winner2);
                    }
                    // current maximum interval is less than (new maximum interval)
                    else if (max[0].Interval < interval)
                    {
                        // clear the list before adding the new one
                        max.Clear();
                        AddInterval(max, producer.Name, interval, winner1, winner2);
                    }
                }
            }
            
            return new AwardsIntervalResult(min, max);
        }

        private static void AddInterval(List<AwardsIntervalResult.AwardsInterval> list, string producer, int interval, Movie winner1, Movie winner2)
        {
            list.Add(new AwardsIntervalResult.AwardsInterval(producer, interval, winner1.Year, winner2.Year));
        }
    }
}
