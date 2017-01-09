using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Objects.Business
{
    public class ClassForInclusions
    {
        public Guid Id;
        public string AString;
        public int AnInt;

        public static ClassForInclusions Create()
        {
            return new ClassForInclusions()
            {
                Id = Guid.NewGuid(),
                AString = Guid.NewGuid().ToString(),
                AnInt = 5
            };
        }
    }
}
