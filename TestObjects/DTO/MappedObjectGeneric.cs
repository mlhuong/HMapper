using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class MappedObjectGeneric<T>
    {
        public int Id;
        public string AString;
        public T GenericProperty;

        public MappedObjectGeneric() { }
        public MappedObjectGeneric(Business.MappedObjectGeneric<Business.VerySimpleClass> source)
        {
            Id = source.Id;
            AString = source.AString;
            GenericProperty = (T)(object)new VerySimpleClass(source.GenericProperty);
        }

        public MappedObjectGeneric(Business.MappedObjectGeneric<Business.PolymorphicBaseClass> source)
        {
            Id = source.Id;
            AString = source.AString;
            GenericProperty = (T)(object)PolymorphicBaseClass.GetMostConcrete(source.GenericProperty);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(MappedObjectGeneric<T>)) return false;
            var target = obj as MappedObjectGeneric<T>;
            return Id==target.Id
                && AString == target.AString
                && GenericProperty.NullOrEquals(target.GenericProperty);
        }
    }
}
