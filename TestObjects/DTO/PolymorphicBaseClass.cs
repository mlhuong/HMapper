using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class PolymorphicBaseClass : IPolymorphic
    {
        public int Id;
        public string AString;

        public PolymorphicBaseClass()
        { }

        public PolymorphicBaseClass(Business.PolymorphicBaseClass source)
        {
            Id = source.Id;
            AString = source.AString;
        }

        public static PolymorphicBaseClass GetMostConcrete(Business.PolymorphicBaseClass source)
        {
            if (source is Business.PolymorphicSubClass)
                return new PolymorphicSubClass((Business.PolymorphicSubClass)source);
            return new PolymorphicBaseClass(source);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(PolymorphicBaseClass)) return false;
            var target = obj as PolymorphicBaseClass;
            return target.Id == Id && AString==target.AString;
        }
    }
}
