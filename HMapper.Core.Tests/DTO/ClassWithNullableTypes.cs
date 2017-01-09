using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMapper.Tests.DTO
{
    public class ClassWithNullableTypes
    {
        public Guid Id;
        public int Int1;
        public int Int2;
        public int? Int3;
        public int? Int4;

        public ClassWithNullableTypes() { }

        public ClassWithNullableTypes(Business.ClassWithNullableTypes source)
        {
            Id = source.Id;
            Int1 = source.Int1.GetValueOrDefault();
            Int2 = source.Int2.GetValueOrDefault();
            Int3 = source.Int3;
            Int4 = source.Int4;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ClassWithNullableTypes)) return false;
            var target = obj as ClassWithNullableTypes;
            return Id == target.Id
                && Int1 == target.Int1
                && Int2 == target.Int2
                && Int3 == target.Int3
                && Int4 == target.Int4;
        }

    }
}
