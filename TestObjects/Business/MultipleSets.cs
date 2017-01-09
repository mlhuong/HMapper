using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class MultipleSets
    {
        public int Id { get; set; }
        public List<SimpleClass> SimpleClasses { get; set; }
        public List<string> StringSet { get; set; }
        public int[] IntegerSet { get; set; }
        public ICollection<double> DoubleSet { get; set; }
        public HashSet<DateTime> DateSet { get; set; }

        public static MultipleSets Create(int nbItems)
        {
            Random rnd = new Random();
            return new MultipleSets()
            {
                Id = rnd.Next(),
                SimpleClasses = Enumerable.Range(1, nbItems).Select(i => SimpleClass.Create(i)).ToList(),
                StringSet = Enumerable.Range(1, nbItems).Select(i => Guid.NewGuid().ToString()).ToList(),
                IntegerSet = Enumerable.Range(1, nbItems).Select(i => rnd.Next()).ToArray(),
                DoubleSet = new Collection<double>(Enumerable.Range(1, nbItems).Select(i => rnd.NextDouble()).ToList()),
                DateSet = new HashSet<DateTime>(Enumerable.Range(1, nbItems).Select(i => DateTime.Now))
            };
            
        }

        public static MultipleSets[] CreateMany(int nb, int nbItems)
        {
            return Enumerable.Range(1, nb).Select(i => Create(nbItems)).ToArray();
        }
    }
}
