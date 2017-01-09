using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class SimpleGeneric<T>
    {
        public int Id;
        public T GenericProperty;

        public SimpleGeneric() { }

        public SimpleGeneric(Business.SimpleGeneric<T> source)
        {
            Id = source.Id;
            GenericProperty = source.GenericProperty;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(SimpleGeneric<T>)) return false;
            var target = obj as SimpleGeneric<T>;
            return Id == target.Id && GenericProperty.NullOrEquals(target.GenericProperty);
        }
    }
}
