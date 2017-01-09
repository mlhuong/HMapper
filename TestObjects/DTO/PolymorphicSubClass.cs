using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestObjects.DTO
{
    public class PolymorphicSubClass : PolymorphicBaseClass
    {
        public string Name;

        public PolymorphicSubClass()
        { }

        public PolymorphicSubClass(Business.PolymorphicSubClass source)
            :base(source)
        {
            Name = source.Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(PolymorphicSubClass)) return false;
            var target = obj as PolymorphicSubClass;
            return Id == target.Id
                && target.Name == Name
                && target.AString == AString;
        }
    }

    /// <summary>
    /// unmapped sub class
    /// </summary>
    public class PolymorphicSubSubClass : PolymorphicSubClass
    { }
}
