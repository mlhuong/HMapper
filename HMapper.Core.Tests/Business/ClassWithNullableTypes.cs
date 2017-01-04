using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMapper.Tests.Business
{
    public class ClassWithNullableTypes
    {
        public Guid Id;
        public int? Int1;
        public int? Int2;
        public int? Int3;
        public int Int4;

        public static ClassWithNullableTypes Create()
        {
            return new ClassWithNullableTypes()
            {
                Id = Guid.NewGuid(),
                Int1 = null,
                Int2 = 2,
                Int3 = 3,
                Int4 = 4
            };
        }
    }
}
