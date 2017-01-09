using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.Business
{
    public class MultipleGenerics<A,B>
    {
        public int Id { get; set; }
        public A A_Property { get; set; }
        public B B_Property { get; set; }

        public static MultipleGenerics<A,B> Create(int id, A A_Value, B B_Value)
        {
            return new MultipleGenerics<A,B>()
            {
                Id = id,
                A_Property = A_Value,
                B_Property = B_Value
            };
        }

        public static MultipleGenerics<A,B>[] CreateMany(Tuple<A,B>[] values)
        {
            return values.Select((t, i) => Create(i, t.Item1, t.Item2)).ToArray();
        }
    }
}
