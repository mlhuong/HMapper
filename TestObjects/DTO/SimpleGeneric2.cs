using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class SimpleGeneric2<T>
    {
        public int Id;
        public T AnotherGenericProperty;
        public string ToBeIngored;

        public SimpleGeneric2() { }

        public SimpleGeneric2(Business.SimpleGeneric2<T> source)
        {
            Id = source.Id;
            AnotherGenericProperty = source.GenericProperty;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(SimpleGeneric2<T>)) return false;
            var target = obj as SimpleGeneric2<T>;
            return Id == target.Id
                && AnotherGenericProperty.NullOrEquals(target.AnotherGenericProperty)
                && ToBeIngored.NullOrEquals(target.ToBeIngored);
        }
    }
}
