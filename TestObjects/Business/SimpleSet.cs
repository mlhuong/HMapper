using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class SimpleSet
    {
        public IList<VerySimpleClass> VerySimpleClasses { get; set; }

        public static SimpleSet Create(int nbItems)
        {
            return new SimpleSet()
            {
                VerySimpleClasses = Enumerable.Range(0, nbItems).Select(i => VerySimpleClass.Create()).ToList()
            };
        }

        public static SimpleSet[] CreateMany(int nb, int nbItems)
        {
            return Enumerable.Range(1, nb).Select(i => Create(nbItems)).ToArray();
        }
    }
}
