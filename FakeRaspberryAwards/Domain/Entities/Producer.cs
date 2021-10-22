using System.Collections.Generic;

namespace FakeRaspberryAwards.Domain.Entities
{
    public class Producer
    {
        public Producer()
        {
            Movies = new List<Movie>();
        }

        public virtual int Id { get; private set; }
        public virtual string Name { get; set; }

        public virtual List<Movie> Movies { get; }
    }
}
