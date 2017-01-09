using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class MappedObjectGeneric<T>
    {
        public int Id { get; set; }
        public string AString { get; set; }
        public T GenericProperty { get; set; }

        public static MappedObjectGeneric<T> Create(int id, T genValue)
        {
            return new MappedObjectGeneric<T>()
            {
                Id = id,
                AString = Guid.NewGuid().ToString(),
                GenericProperty = genValue
            };
        }

        public static MappedObjectGeneric<T>[] CreateMany(T[] genValues)
        {
            return genValues.Select((t, i) => Create(i, t)).ToArray();
        }
    }
}
