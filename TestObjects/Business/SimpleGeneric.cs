using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class SimpleGeneric<T>
    {
        public int Id { get; set; }
        public T GenericProperty { get; set; }

        public static SimpleGeneric<T> Create(int id, T genValue)
        {
            return new SimpleGeneric<T>()
            {
                Id = id,
                GenericProperty = genValue
            };
        }

        public static SimpleGeneric<T>[] CreateMany(T[] genValues)
        {
            return genValues.Select((t, i) => Create(i, t)).ToArray();
        }
    }
}
