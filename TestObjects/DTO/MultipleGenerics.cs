using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class MultipleGenerics<B,A>
    {
        public int Id;
        public A A_Property;
        public B B_Property;

        public MultipleGenerics() { }

        public MultipleGenerics(Business.MultipleGenerics<A,B> source)
        {
            Id = source.Id;
            A_Property = source.A_Property;
            B_Property = source.B_Property;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(MultipleGenerics<B, A>)) return false;
            var target = obj as MultipleGenerics<B,A>;
            return Id == target.Id 
                && A_Property.NullOrEquals(target.A_Property)
                && B_Property.NullOrEquals(target.B_Property);
        }
    }
}
