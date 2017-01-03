using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Business
{
    public class SimpleGeneric2<T>
    {
        public int Id { get; set; }
        public T GenericProperty { get; set; }
        public string ToBeIngored;

        public static SimpleGeneric2<T> Create(int id, T genValue)
        {
            return new SimpleGeneric2<T>()
            {
                Id = id,
                GenericProperty = genValue,
                ToBeIngored = "dummy"
            };
        }

        public static SimpleGeneric2<T>[] CreateMany(T[] genValues)
        {
            return genValues.Select((t, i) => Create(i, t)).ToArray();
        }
    }
}
