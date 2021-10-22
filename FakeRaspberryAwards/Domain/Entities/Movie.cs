using System.Collections.Generic;

namespace FakeRaspberryAwards.Domain.Entities
{
    public class Movie
    {
        public Movie()
        {
            Studios = new List<Studio>();
            Producers = new List<Producer>();
        }

        public virtual int Id { get; private set; }
        public virtual int Year { get; set; }
        public virtual string Title { get; set; }
        public virtual bool Winner { get; set; }

        public virtual List<Studio> Studios { get; }
        public virtual List<Producer> Producers { get; }
    }
}
