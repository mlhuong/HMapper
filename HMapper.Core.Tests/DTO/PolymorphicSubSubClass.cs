using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.DTO
{
    public class PolymorphicSubSubClass : PolymorphicSubClass
    {
        public Guid Guid;

        public PolymorphicSubSubClass()
        { }

        public PolymorphicSubSubClass(Business.PolymorphicSubSubClass source)
            :base(source)
        {
            Guid = source.Guid;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(PolymorphicSubSubClass)) return false;
            var target = obj as PolymorphicSubSubClass;
            return Id == target.Id
                && target.Name == Name
                && target.AString == AString
                && target.Guid == Guid;
        }
    }
}
